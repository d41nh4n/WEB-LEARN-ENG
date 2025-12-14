namespace Allen.Application;

[RegisterService(typeof(IVocabularyService))]
public class VocabularyService(
    IVocabularyMeaningService _vocabularyMeaningService,
    IMeiliSearchService<VocabularyMLSModel> _meiliSearchService,
    IGeminiService _geminiService,
    IVocabularyRepository _vocabularyRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IVocabularyService
{
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
    public async Task<OperationResult> CreateMultiAsync(AddMultipleVocabularyModel vocabularyModels)
    {
        var vocabularyRepo = _unitOfWork.Repository<VocabularyEntity>();
        var topicRepo = _unitOfWork.Repository<TopicEntity>();

        // 1. Normalize + chống duplicate trong batch
        var normalizedVocabularies = vocabularyModels.Words
            .GroupBy(v => StringExtensions.ConvertToCase(v.Word, StringCaseType.Lower))
            .Select(g =>
            {
                var model = g.First();
                model.Word = StringExtensions.ConvertToCase(model.Word, StringCaseType.Lower);
                return model;
            })
            .ToList();

        // 2. Check duplicate trong DB
        var words = normalizedVocabularies.Select(x => x.Word).ToList();

        var existedWords = await vocabularyRepo
            .GetListByConditionAsync(v => words.Contains(v.Word));

        if (existedWords.Any())
        {
            var existedWordList = string.Join(", ", existedWords.Select(x => x.Word));

            return OperationResult.Failure(
                ErrorMessageBase.Format(
                    ErrorMessageBase.AlreadyExists,
                    nameof(VocabularyEntity),
                    existedWordList));
        }

        // 3. Check topic tồn tại (distinct)
        var topicIds = normalizedVocabularies
            .Select(x => x.TopicId)
            .Distinct()
            .ToList();

        foreach (var topicId in topicIds)
        {
            if (!await topicRepo.CheckExistByIdAsync(topicId))
            {
                return OperationResult.Failure(
                    ErrorMessageBase.Format(
                        ErrorMessageBase.NotFound,
                        nameof(TopicEntity),
                        topicId));
            }
        }

        var createdVocabularies = new List<VocabularyEntity>();

        // 4. Transaction
        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            foreach (var vocabularyModel in normalizedVocabularies)
            {
                // 4.1 Create vocabulary
                var createdVocabulary =
                    await CreateVocabularyAsync(vocabularyModel, vocabularyModel.TopicId);

                // 4.2 Create meanings
                var meaningResult =
                    await _vocabularyMeaningService.CreateVocabularyMeaningsAsync(
                        createdVocabulary.Id,
                        vocabularyModel.VocabularyMeaningModels);

                if (!meaningResult.Success)
                    throw new InternalServerException(
                        ErrorMessageBase.CreateFailure,
                        nameof(VocabularyMeaningEntity));

                createdVocabularies.Add(createdVocabulary);
            }
        });

        // 5. Return result
        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(
                ErrorMessageBase.CreatedSuccess,
                nameof(VocabularyEntity)),
            createdVocabularies.Select(v => v.Word).ToList());
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

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            _mapper.Map(updateModel, existingVocab);
            existingVocab.Word = wordNormalize;

            var statusUpdateMeaning = await _vocabularyMeaningService.UpdateVocabularyMeaningsAsync(vocabId, updateModel.VocabularyMeaningModels);

            vocabularyRepo.UpdateAsync(existingVocab);
        });


        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(VocabularyEntity)));
    }
    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        if (!await _vocabularyRepository.CheckExistByIdAsync(id))
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity), id));
        }
        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            await _vocabularyMeaningService.DeleteVocabMeaningByVocabIdAsync(id);
            await _vocabularyRepository.DeleteByIdAsync(id);
        });

        await _meiliSearchService.DeleteAsync(id);

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(VocabularyEntity)), id);
    }
    public async Task<List<VocabularyEntity>> GetVocabulariesByIdsAsync(List<Guid> vocabIds)
    {
        return await _vocabularyRepository.GetVocabulariesByIdsAsync(vocabIds);
    }


    public async Task<QuizVocabulariesResponeModel> GetQuizVocabulariesAsync(QuizVocabulariesRequestModel model)
    {
        return await _vocabularyRepository.GetQuizVocabulariesAsync(model);
    }

    public async Task<QueryResult<VocabularyModel>> GetVocabulariesByTopicIdAsync(Guid topic, QueryInfo queryInfo)
    {
        return await _vocabularyRepository.GetVocabulariesByTopicIdAsync(queryInfo, topic);
    }

    public async Task<VocabularyGenerateResponseModel> BuildVocabularyByWordsAsync(
    WordVocabularyForGenerateModel request)
    {
        var response = new VocabularyGenerateResponseModel();

        // 1. Normalize input
        var inputWords = request.Words
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Select(w => w.Trim().ToLower())
            .Distinct()
            .ToList();

        if (inputWords.Count == 0)
            return response;

        // 2. Existing vocab
        var existingEntities =
            await _vocabularyRepository.GetVocabulariesByWordsAsync(inputWords);

        var existingWords = existingEntities
            .Select(v => v.Word.ToLower())
            .ToHashSet();

        response.Existing = _mapper.Map<List<VocabularyPreviewModel>>(existingEntities);

        // 3. Words cần generate
        var newWords = inputWords
            .Where(w => !existingWords.Contains(w))
            .ToList();

        if (newWords.Count == 0)
            return response;

        // 4. Call AI
        var aiRawText = await _geminiService.BuildVocabularyAsync(newWords);

        if (string.IsNullOrWhiteSpace(aiRawText))
            throw new InternalServerException(ErrorMessageBase.CreateFailure, "Gemini returned empty response");

        List<VocabularyGenerateAiModel> aiModels;
        try
        {
            aiModels = JsonSerializer.Deserialize<List<VocabularyGenerateAiModel>>(
                aiRawText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? [];
        }
        catch
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, "Invalid AI JSON format");
        }

        // 5. Load topic map (1 query)
        var topicNames = aiModels
            .Where(x => !x.Skip)
            .Select(x => x.TopicName)
            .Distinct()
            .ToList();

        var topics = await _unitOfWork
            .Repository<TopicEntity>()
            .GetListByConditionAsync(t => topicNames.Contains(t.TopicName));

        var topicMap = topics.ToDictionary(
            t => t.TopicName,
            t => t,
            StringComparer.OrdinalIgnoreCase
);
        // 6. Map AI → TEMP entity
        var tempEntities = new List<VocabularyEntity>();

        foreach (var ai in aiModels)
        {
            if (ai.Skip)
            {
                response.Invalid.Add(new InvalidVocabularyGenerateModel
                {
                    Word = ai.Word,
                    Reason = "AI skipped this word"
                });
                continue;
            }

            if (!topicMap.TryGetValue(ai.TopicName, out var topic))
            {
                response.Invalid.Add(new InvalidVocabularyGenerateModel
                {
                    Word = ai.Word!,
                    Reason = $"Topic '{ai.TopicName}' not found"
                });
                continue;
            }

            tempEntities.Add(MapFromAi(ai, topic));
        }

        // 7. Map Entity → Model (preview)
        response.Generated = _mapper.Map<List<VocabularyPreviewModel>>(tempEntities);

        return response;
    }


    public VocabularyEntity MapFromAi(
    VocabularyGenerateAiModel ai,
    TopicEntity topic)
    {
        return new VocabularyEntity
        {
            Id = Guid.Empty, // Temporary
            Word = ai.Word!.Trim().ToLower(),
            TopicId = topic.Id,
            Topic = topic,
            Level = ParseLevel(ai.Level),

            VocabularyMeanings = ai.VocabularyMeaningModels?
                .Where(m => !string.IsNullOrWhiteSpace(m.DefinitionEN))
                .Select(m => new VocabularyMeaningEntity
                {
                    Id = Guid.Empty,
                    PartOfSpeech = ParsePartOfSpeech(m.PartOfSpeech),
                    Pronunciation = m.Pronunciation,
                    DefinitionEN = m.DefinitionEN,
                    DefinitionVN = m.DefinitionVN,
                    Example1 = m.Example1,
                    Example2 = m.Example2
                })
                .ToList()
                ?? []
        };
    }

    private PartOfSpeechType? ParsePartOfSpeech(string? pos)
    {
        if (string.IsNullOrWhiteSpace(pos))
            return null;

        if (Enum.TryParse<PartOfSpeechType>(
            pos,
            ignoreCase: true,
            out var result))
        {
            return result;
        }

        return null;
    }
    private LevelType ParseLevel(string? level)
    {
        if (Enum.TryParse<LevelType>(
            level,
            ignoreCase: true,
            out var result))
        {
            return result;
        }

        return LevelType.B1;
    }

}
