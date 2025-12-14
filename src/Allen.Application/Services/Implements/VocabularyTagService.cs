namespace Allen.Application;

[RegisterService(typeof(IVocabularyTagService))]
public class VocabularyTagService(
    IVocabularyTagRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IVocabularyTagService
{
    // =========================
    // READ
    // =========================
    public async Task<IEnumerable<VocabularyTagModel>> GetVocabularyTagsModelByVocabularyIdAsync(Guid vocabularyId)
    {
        var entities = await _repository.GetVocabularyTagByVocabIdAsync(vocabularyId);
        return _mapper.Map<List<VocabularyTagModel>>(entities);
    }

    public async Task<IEnumerable<VocabularyTagEntity>> GetVocabularyTagsEntityByVocabularyIdAsync(Guid vocabularyId)
    {
        return await _repository.GetVocabularyTagByVocabIdAsync(vocabularyId);
    }

    // =========================
    // CREATE single
    // =========================
    public async Task<OperationResult> AddVocabularyTagToVocabularyAsync(Guid vocabularyId, Guid tagId)
    {
        var entity = new VocabularyTagEntity
        {
            Id = Guid.NewGuid(),
            VocabularyId = vocabularyId,
            TagId = tagId
        };

        await _unitOfWork.Repository<VocabularyTagEntity>().AddAsync(entity);

        return OperationResult.SuccessResult(ErrorMessageBase.CreatedSuccess, entity.Id);
    }

    // =========================
    // CREATE multiple
    // =========================
    public async Task<OperationResult> CreateVocabularyTagsAsync(Guid vocabularyId, List<Guid> tags)
    {
        tags = tags.Distinct().ToList();

        var vocabularyTagEntities = tags.Select(tagId => new VocabularyTagEntity
        {
            Id = Guid.NewGuid(),
            VocabularyId = vocabularyId,
            TagId = tagId,
        }).ToList();

        await _unitOfWork.Repository<VocabularyTagEntity>().AddRangeAsync(vocabularyTagEntities);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(VocabularyTagEntity));
        }

        return OperationResult.SuccessResult(
            ErrorMessageBase.CreatedSuccess,
            vocabularyTagEntities.Select(x => x.TagId)
        );
    }


    // =========================
    // UPDATE (remove old + add new)
    // =========================
    public async Task<OperationResult> UpdateVocabularyTagsAsync(Guid vocabularyId, List<Guid> tags)
    {
        tags = tags.Distinct().ToList();
        // Lấy tất cả VocabularyTag entity hiện có cho vocab
        var existedEntities = await _repository.GetVocabularyTagByVocabIdAsync(vocabularyId);
        var existedTags = existedEntities.Select(e => e.TagId).ToList();

        // Tags cần thêm mới
        var tagsToAdd = tags.Except(existedTags).ToList();

        // Tags cần xóa
        var tagsToRemove = existedEntities
            .Where(e => !tags.Contains(e.TagId))
            .ToList();

        // Thêm mới
        if (tagsToAdd.Count > 0)
        {
            await AddTagsExistedIntoVocabularyAsync(tagsToAdd, vocabularyId);
        }

        // Xóa bớt (xóa trực tiếp entity, không query lại)
        if (tagsToRemove.Count > 0)
        {
            _unitOfWork.Repository<VocabularyTagEntity>().DeleteRangeAsync(tagsToRemove);
        }

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.UpdateFailure, nameof(VocabularyTagEntity));
        }

        return OperationResult.SuccessResult(ErrorMessageBase.UpdatedSuccess);
    }

    // =========================
    // DELETE all tags from 1 vocabulary
    // =========================
    public async Task<OperationResult> RemoveAllVocabularyTagsFromVocabularyAsync(Guid vocabularyId)
    {
        var vocabularyTags = await GetVocabularyTagsEntityByVocabularyIdAsync(vocabularyId);

        if (!vocabularyTags.Any())
        {
            return OperationResult.SuccessResult(ErrorMessageBase.DeletedSuccess, 0);
        }

        _unitOfWork.Repository<VocabularyTagEntity>().DeleteRangeAsync(vocabularyTags);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.DeleteFailure, nameof(VocabularyTagEntity));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.DeletedSuccess, vocabularyTags.Count());
    }

    // =========================
    // DELETE a tag from all vocabularies
    // =========================
    public async Task<OperationResult> RemoveVocabularyTagFromAllVocabulariesAsync(Guid tagId)
    {
        try
        {
            var repo = _unitOfWork.Repository<VocabularyTagEntity>();

            var allWithTag = await repo.GetListByConditionAsync(x => x.TagId == tagId);

            if (allWithTag.Count == 0)
                throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyTagEntity), tagId));

            repo.DeleteRangeAsync(allWithTag);
            if (!await _unitOfWork.SaveChangesAsync())
            {
                throw new InternalServerException(ErrorMessageBase.DeleteFailure, nameof(VocabularyTagEntity));
            }
            return OperationResult.SuccessResult(ErrorMessageBase.DeletedSuccess, allWithTag.Count);
        }
        catch (Exception ex)
        {
            throw new InternalServerException(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(VocabularyTagEntity)), ex.Message);
        }
    }

    // =========================
    // Add tags into a vocabulary
    // =========================
    public async Task<OperationResult> AddTagsExistedIntoVocabularyAsync(List<Guid> tagsId, Guid vocabularyId)
    {
        tagsId = tagsId.Distinct().ToList();
        var listVocabularyTagEntity = tagsId.Select(x => new VocabularyTagEntity
        {
            Id = Guid.NewGuid(),
            VocabularyId = vocabularyId,
            TagId = x
        }).ToList();

        var addStatus = await _unitOfWork.
            Repository<VocabularyTagEntity>().AddRangeAsync(listVocabularyTagEntity);

        if (!addStatus || !await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(VocabularyTagEntity));
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(VocabularyTagEntity)), listVocabularyTagEntity);
    }

    // =========================
    // Get all tagId in a vocabulary
    // =========================
    public async Task<List<Guid>> GetTagsIdByVocabularyIdAsync(Guid vocabularyId)
    {
        try
        {
            var tags = await _repository.GetTagsIdByVocabularyIdAsync(vocabularyId);
            return tags?.ToList() ?? [];
        }
        catch (Exception ex)
        {
            throw new InternalServerException(
                ErrorMessageBase.Format(ErrorMessageBase.GetFailure, nameof(VocabularyTagEntity)),
                ex.Message
            );
        }
    }

    public Task<OperationResult> RemoveVocabularyTagsFromVocabularyAsync(List<Guid> tagsId, Guid vocabularyId)
    {
        throw new NotImplementedException();
    }
}
