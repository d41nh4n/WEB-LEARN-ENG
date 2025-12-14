
namespace Allen.Application;

[RegisterService(typeof(IVocabularyService))]
public class VocabularyService(
    ITagService _tagService,
    IVocabularyTagService _vocabularyTagService,
    IVocabularyMeaningService _vocabularyMeaningService,
    IMeiliSearchService<VocabularyMLSModel> _meiliSearchService,
    IVocabularyRepository _vocabularyRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IVocabularyService
{
    // =========================
    // READ
    // =========================
    public Task<QueryResult<VocabulariesModel>> GetVocabulariesAsync(QueryInfo queryInfo)
        => _vocabularyRepository.GetVocabulariesAsync(queryInfo);

    public async Task<VocabularyModel> GetVocabularyByIdAsync(Guid vocabId)
    {
        if (vocabId == Guid.Empty)
            throw new BadRequestException(ErrorMessageBase.Format(ErrorMessageBase.Required, nameof(vocabId)));

        return await _vocabularyRepository.GetVocabularyByIdAsync(vocabId)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity))); ;
    }

    public async Task<VocabularyModel> GetVocabularyByWordAsync(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            throw new BadRequestException(ErrorMessageBase.Format(ErrorMessageBase.Required, nameof(word)));

        return await _vocabularyRepository.GetVocabularyByWordAsync(word);
    }

    // =========================
    // CREATE
    // =========================
    public async Task<OperationResult> CreateAsync(CreateVocabularyModel vocabularyModel)
    {
        var wordNormalize = StringExtensions.ConvertToCase(vocabularyModel.Word, StringCaseType.Lower);

        var vocabularyRepo = _unitOfWork.Repository<VocabularyEntity>();

        // 🔹 Check vocabulary exists
        if (await vocabularyRepo.CheckExistAsync(x => x.Word == wordNormalize))
        {
            return OperationResult.Failure(
                ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(VocabularyEntity), wordNormalize));
        }

        // 🔹 Check topic exists
        if (!await _unitOfWork.Repository<TopicEntity>().CheckExistByIdAsync(vocabularyModel.TopicId))
        {
            return OperationResult.Failure(
                ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TopicEntity), vocabularyModel.TopicId));
        }

        // 🔹 Check tags exist
        var lstTagNotExisted = await _tagService.CheckNotExistedTagAsync(vocabularyModel.TagsId);
        if (lstTagNotExisted.Count > 0)
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TagEntity), lstTagNotExisted));
        }

        VocabularyEntity? createdVocabulary = null;

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            // 1. Create vocabulary
            createdVocabulary = await CreateVocabularyAsync(vocabularyModel, vocabularyModel.TopicId);

            // 2. Add meanings
            var meaningResult = await _vocabularyMeaningService.CreateVocabularyMeaningsAsync(
                createdVocabulary.Id, vocabularyModel.VocabularyMeaningModels);
            if (!meaningResult.Success) throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(VocabularyMeaningEntity));

            // 3. Add tags
            var tagResult = await _vocabularyTagService.AddTagsExistedIntoVocabularyAsync(
                vocabularyModel.TagsId, createdVocabulary.Id);
            if (!tagResult.Success) throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(VocabularyMeaningEntity));
        });

        //await _meiliSearchService.AddVocabularyAsync(new VocabularyMLSModel
        //{
        //    Word = createdVocabulary!.Word,
        //    Id = createdVocabulary.Id,
        //});

        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(VocabularyEntity)),
            createdVocabulary!.Word);
    }

    private async Task<VocabularyEntity> CreateVocabularyAsync(CreateVocabularyModel model, Guid topicId)
    {
        var vocabularyEntity = _mapper.Map<VocabularyEntity>(model);
        vocabularyEntity.TopicId = topicId;

        vocabularyEntity = await _unitOfWork.Repository<VocabularyEntity>().AddAsync(vocabularyEntity);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(VocabularyEntity));
        }

        return vocabularyEntity;
    }

    // =========================
    // UPDATE
    // =========================
    public async Task<OperationResult> UpdateAsync(UpdateVocabularyModel updateModel, Guid vocabId)
    {
        var wordNormalize = StringExtensions.ConvertToCase(updateModel.Word, StringCaseType.Lower);
        var vocabularyRepo = _unitOfWork.Repository<VocabularyEntity>();

        var existingVocab = await vocabularyRepo.GetByIdAsync(vocabId);
        if (existingVocab == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity), vocabId));

        // 🔹 Check if another vocab with same word exists
        if (await vocabularyRepo.CheckExistAsync(x => x.Word == wordNormalize && x.Id != vocabId))
        {
            return OperationResult.Failure(
                ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(VocabularyEntity), updateModel.Word));
        }

        // 🔹 Check topic
        if (!await _unitOfWork.Repository<TopicEntity>().CheckExistByIdAsync(updateModel.TopicId))
        {
            return OperationResult.Failure(
                ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TopicEntity), updateModel.TopicId));
        }

        // 🔹 Check tags
        if (updateModel.UpdateTagsId.Count() > 0)
        {
            var lstTagNotExisted = await _tagService.CheckNotExistedTagAsync(updateModel.UpdateTagsId);
            if (lstTagNotExisted.Count > 0)
            {
                return OperationResult.Failure(
                    ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TagEntity), lstTagNotExisted));
            }
        }
        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            // Update main fields
            _mapper.Map(updateModel, existingVocab);
            existingVocab.Word = wordNormalize;

            // Update Tags
            if (updateModel.UpdateTagsId.Count() > 0)
            {
                var statusUpdateTag = await _vocabularyTagService.UpdateVocabularyTagsAsync(vocabId, updateModel.UpdateTagsId);
            }
            else
            {
                await _vocabularyTagService.RemoveAllVocabularyTagsFromVocabularyAsync(vocabId);
            }
            // Update Meanings
            var statusUpdateMeaning = await _vocabularyMeaningService.UpdateVocabularyMeaningsAsync(vocabId, updateModel.VocabularyMeaningModels);

            // Update entity
            vocabularyRepo.UpdateAsync(existingVocab);
        });

        //await _meiliSearchService.UpdateVocabularyAsync(new VocabularyMLSModel
        //{
        //    Word = updateModel!.Word,
        //    Id = vocabId,
        //});

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(VocabularyEntity)));
    }

    // =========================
    // DELETE
    // =========================
    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        if (!await _vocabularyRepository.CheckExistByIdAsync(id))
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity), id));
        }
        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            await _vocabularyMeaningService.DeleteVocabMeaningByVocabIdAsync(id);
            await _vocabularyTagService.RemoveAllVocabularyTagsFromVocabularyAsync(id);
            await _vocabularyRepository.DeleteByIdAsync(id);
        });

        await _meiliSearchService.DeleteAsync(id);

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(VocabularyEntity)), id);
    }

    ///======================== Basics Functions ========================///

    public async Task<List<VocabularyEntity>> GetVocabulariesByIdsAsync(List<Guid> vocabIds)
    {
        return await _vocabularyRepository.GetVocabulariesByIdsAsync(vocabIds);
    }

    ///======================== Advanced Functions ========================///

    public async Task<QuizVocabulariesResponeModel> GetQuizVocabulariesAsync(QuizVocabulariesRequestModel model)
    {
        return await _vocabularyRepository.GetQuizVocabulariesAsync(model);
    }

    public async Task<QueryResult<VocabularyModel>> GetVocabulariesByTopicIdAsync(Guid topic, QueryInfo queryInfo)
    {
        return await _vocabularyRepository.GetVocabulariesByTopicIdAsync(queryInfo, topic);
    }
}
