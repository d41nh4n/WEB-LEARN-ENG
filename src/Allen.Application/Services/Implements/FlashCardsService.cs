namespace Allen.Application;

[RegisterService(typeof(IFlashCardsService))]
public class FlashCardsService
    (IFlashCardsStateService _flashCardStateService,
    IDeckService _deckService,
    IReviewFLHistoryService _reviewFLHistoryService,
    IVocabularyService _vocabularyService,
    IBlobStorageService _blobStorageService,
    IReviewFLHistoryService reviewFLHistoryService,
    ISRSService _srsService,
    IRedisService _redisService,
    IUnitOfWork _unitOfWork,
    IFlashCardsRepository _flashCardsRepository,
    IMapper _mapper) : IFlashCardsService
{
    public async Task<OperationResult> CreateAsync(CreateOrUpdateFlashCardWithFileModel model, Guid userId, Guid deckId)
    {
        if (deckId == Guid.Empty || !await _unitOfWork.Repository<DeckEntity>().CheckExistByIdAsync(deckId))
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, deckId, nameof(DeckEntity)));

        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        Guid idFlashCard = Guid.NewGuid();

        var newlyUploadedUrls = new List<string>();

        var frontImgUrl = await UploadFileIfPresentAsync(model.FrontImgFile, newlyUploadedUrls);
        var backImgUrl = await UploadFileIfPresentAsync(model.BackImgFile, newlyUploadedUrls);
        var frontAudioUrl = await UploadFileIfPresentAsync(model.FrontAudioFile, newlyUploadedUrls);
        var backAudioUrl = await UploadFileIfPresentAsync(model.BackAudioFile, newlyUploadedUrls);

        var frontContentJson = CreateContentJson(model.TextFrontCard!, frontImgUrl, frontAudioUrl);
        var backContentJson = CreateContentJson(model.TextBackCard!, backImgUrl, backAudioUrl);

        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var entity = new FlashCardEntity
                {
                    Id = idFlashCard,
                    DeckId = deckId,
                    FrontContent = frontContentJson,
                    BackContent = backContentJson,
                    Hint = model.Hint,
                    PersonalNotes = model.PersonalNotes
                };

                await _unitOfWork.Repository<FlashCardEntity>().AddAsync(entity);

                if (!await _unitOfWork.SaveChangesAsync())
                {
                    throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(FlashCardEntity));
                }

                var initialState = FlashCardStateModel.CreateNew(entity.Id);
                await _flashCardStateService.CreateAsync(initialState);
            });
        }
        catch (Exception)
        {
            await CleanUpUploadedFilesAsync(newlyUploadedUrls);
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(FlashCardEntity)), idFlashCard);
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(FlashCardEntity)), idFlashCard);
    }

    public async Task<OperationResult> CreateMultiAsync(FlashCardCreateMultiRequestModel models, Guid userId, Guid deckId)
    {
        if (deckId == Guid.Empty || !await _unitOfWork.Repository<DeckEntity>().CheckExistByIdAsync(deckId))
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, deckId, nameof(deckId)));

        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        var newCardIds = new List<Guid>();

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            List<FlashCardEntity> entityList = new();
            List<FlashCardStateEntity> entityStateList = new();

            foreach (var model in models.Cards)
            {
                var entity = _mapper.Map<FlashCardEntity>(model);
                entity.DeckId = deckId;
                entity.Id = Guid.NewGuid();
                entityList.Add(entity);

                var initialState = FlashCardStateEntity.CreateNew(entity.Id);
                entityStateList.Add(initialState);

                newCardIds.Add(entity.Id);
            }

            await _unitOfWork.Repository<FlashCardEntity>().AddRangeAsync(entityList);

            await _unitOfWork.Repository<FlashCardStateEntity>().AddRangeAsync(entityStateList);

            if (!await _unitOfWork.SaveChangesAsync())
                OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, "FlashCards and States"), "");
        });

        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, $"{models.Cards.Count} FlashCards"),
            newCardIds
        );
    }

    public async Task<OperationResult> DeleteAsync(Guid id, Guid userId, Guid deckId)
    {
        var entity = await _unitOfWork.Repository<FlashCardEntity>().GetByIdAsync(id);
        if (entity == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, id, nameof(FlashCardEntity)));

        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);


        var urlsToDelete = ExtractAllUrls(entity);


        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                await _unitOfWork.Repository<FlashCardEntity>().DeleteByIdAsync(id);

                if (!await _unitOfWork.SaveChangesAsync())
                {

                    throw new InternalServerException(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(FlashCardEntity)), "Failed to commit changes.");
                }
            });


            await CleanUpUploadedFilesAsync(urlsToDelete);
        }
        catch (Exception ex)
        {

            if (ex is NotFoundException || ex is ForbiddenException) throw;

            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(FlashCardEntity)), ex.Message);
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(FlashCardEntity)));
    }

    public async Task<OperationResult> DeleteMultiAsync(DeleteFlashCardLstModel cardLstModel, Guid userId, Guid deckId)
    {
        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);


        var cardsToDelete = await _flashCardsRepository.GetAllCardsForByIdsAndDeckIdAsync(cardLstModel.FlashCardIds, deckId);

        if (cardsToDelete == null || cardsToDelete.Count == 0)
            return OperationResult.SuccessResult("No cards found to delete.");


        var urlsToDelete = cardsToDelete
            .SelectMany(card => ExtractAllUrls(card))
            .Distinct()
            .ToList();

        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                foreach (var card in cardsToDelete)
                {
                    await _unitOfWork.Repository<FlashCardEntity>().DeleteByIdAsync(card.Id);

                }

                if (!await _unitOfWork.SaveChangesAsync())
                {

                    throw new InternalServerException(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(FlashCardEntity)), "Failed to commit changes for multiple deletions.");
                }
            });

            await CleanUpUploadedFilesAsync(urlsToDelete);
        }
        catch (Exception ex)
        {

            if (ex is ForbiddenException) throw;

            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(FlashCardEntity)), ex.Message);
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, cardsToDelete.Count));
    }

    public async Task<QueryResult<Guid>> GetFlashCardIdsByDeck(QueryInfo queryInfo, Guid deckId, Guid userId)
    {
        if (!await _unitOfWork.Repository<FlashCardEntity>().CheckExistByIdAsync(deckId))
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, deckId, nameof(FlashCardEntity)));

        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        return await _flashCardsRepository.GetFlashCardIdsByDeck(queryInfo, deckId);
    }
    public async Task<FlashCardModel> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new BadRequestException(ErrorMessageBase.Format(ErrorMessageBase.Required, nameof(id)));

        return _mapper.Map<FlashCardModel>(await _flashCardsRepository.GetByIdAsync(id));
    }

    public async Task<List<Guid>> GetListUserIdToNotificateStudyToday()
    {
        return await _flashCardsRepository.GetListUserIdToNotificateStudyToday();
    }


    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateFlashCardWithFileModel model, Guid userId, Guid deckId)
    {

        var entity = await _unitOfWork.Repository<FlashCardEntity>().GetByIdAsync(id);
        if (entity == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, id, nameof(FlashCardEntity)));

        if (entity.DeckId != deckId || !await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        var oldFrontImgUrl = GetOldFileUrl(entity.FrontContent, "Image");
        var oldBackImgUrl = GetOldFileUrl(entity.BackContent, "Image");
        var oldFrontAudioUrl = GetOldFileUrl(entity.FrontContent, "Audio");
        var oldBackAudioUrl = GetOldFileUrl(entity.BackContent, "Audio");

        var newlyUploadedUrls = new List<string>();

        var newFrontImgUrl = await UploadFileIfPresentAsync(model.FrontImgFile, newlyUploadedUrls);
        var newBackImgUrl = await UploadFileIfPresentAsync(model.BackImgFile, newlyUploadedUrls);
        var newFrontAudioUrl = await UploadFileIfPresentAsync(model.FrontAudioFile, newlyUploadedUrls);
        var newBackAudioUrl = await UploadFileIfPresentAsync(model.BackAudioFile, newlyUploadedUrls);

        var finalFrontImgUrl = string.IsNullOrEmpty(newFrontImgUrl) ? oldFrontImgUrl : newFrontImgUrl;
        var finalBackImgUrl = string.IsNullOrEmpty(newBackImgUrl) ? oldBackImgUrl : newBackImgUrl;
        var finalFrontAudioUrl = string.IsNullOrEmpty(newFrontAudioUrl) ? oldFrontAudioUrl : newFrontAudioUrl;
        var finalBackAudioUrl = string.IsNullOrEmpty(newBackAudioUrl) ? oldBackAudioUrl : newBackAudioUrl;


        var newFrontContentJson = CreateContentJson(model.TextFrontCard, finalFrontImgUrl, finalFrontAudioUrl);
        var newBackContentJson = CreateContentJson(model.TextBackCard, finalBackImgUrl, finalBackAudioUrl);

        try
        {
            _mapper.Map(model, entity);

            entity.FrontContent = newFrontContentJson;
            entity.BackContent = newBackContentJson;

            _unitOfWork.Repository<FlashCardEntity>().UpdateAsync(entity);


            if (!await _unitOfWork.SaveChangesAsync())
            {
                throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(FlashCardEntity)));
            }

            await DeleteOldFileIfReplacedAsync(newFrontImgUrl, oldFrontImgUrl);
            await DeleteOldFileIfReplacedAsync(newBackImgUrl, oldBackImgUrl);
            await DeleteOldFileIfReplacedAsync(newFrontAudioUrl, oldFrontAudioUrl);
            await DeleteOldFileIfReplacedAsync(newBackAudioUrl, oldBackAudioUrl);
        }
        catch (Exception ex)
        {
            await CleanUpUploadedFilesAsync(newlyUploadedUrls);

            if (ex is NotFoundException || ex is ForbiddenException) throw;

            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(FlashCardEntity)), ex.Message);
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(FlashCardEntity)), id);
    }

    public async Task<QueryResult<FlashCardStudyQueueLstModel>> GetStudyQueueAsync(Guid deckId, Guid userId)
    {
        var key = $"study-queue-{deckId}";
        //Get in Redis first
        var cachedData = await _redisService.GetAsync<string>(key);

        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedResult = JsonSerializer.Deserialize<FlashCardStudyQueueLstModel>(cachedData);
            return new QueryResult<FlashCardStudyQueueLstModel>
            {
                Data = [cachedResult!],
                TotalCount = cachedResult!.NewFlashCard.Count + cachedResult.ReviewFlashCard.Count
            };
        }
        var deckProps = await _deckService.GetDeckPropsAsync(deckId);

        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        var reviewAndNewCards = await _flashCardsRepository.GetSrsStudyQueueAsync(deckId, DateTime.UtcNow, deckProps.NumberFlashCardsPerSession);

        FlashCardStudyQueueLstModel result = new();

        reviewAndNewCards.ForEach(card =>
        {
            var mappedCard = _mapper.Map<FlashCardStudyModel>(card);
            if (card.CardState!.Repetition == 0)
                result.NewFlashCard.Add(mappedCard);
            else
                result.ReviewFlashCard.Add(mappedCard);
        });

        var serializedResult = JsonSerializer.Serialize(result);
        var ttl = CalculateTtlUntilEndOfDay();

        await _redisService.SetAsync(key, serializedResult, ttl);

        return new QueryResult<FlashCardStudyQueueLstModel>
        {
            Data = [result],
            TotalCount = reviewAndNewCards.Count
        };
    }

    public async Task<OperationResult> ProcessReviewAsync(Guid deckId, Guid flashCardId, ReviewFlashCardRequestModel request, Guid userId)
    {
        var key = $"study-queue-{deckId}";

        var card = await _flashCardsRepository.GetFlashCardByIdAsync(flashCardId)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, flashCardId, nameof(FlashCardEntity)));

        if (!await _deckService.CheckDeckOwner(card.DeckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        var currentState = card.CardState;
        Enum.TryParse<RatingLearningCard>(request.Rating, true, out var rating);

        var cachedData = await _redisService.GetAsync<string>(key);
        FlashCardStudyQueueLstModel studyQueueList = null!;

        if (!string.IsNullOrEmpty(cachedData))
        {
            studyQueueList = JsonSerializer.Deserialize<FlashCardStudyQueueLstModel>(cachedData)!;
        }

        bool isCardFoundInCache = false;
        bool shouldRemoveFromCache = false;


        if (rating == RatingLearningCard.Good || rating == RatingLearningCard.Easy)
        {
            shouldRemoveFromCache = true;
        }

        if (studyQueueList != null && shouldRemoveFromCache)
        {
            var newCard = studyQueueList.NewFlashCard.Find(fc => fc.Id == flashCardId);
            if (newCard != null)
            {
                studyQueueList.NewFlashCard.Remove(newCard);
                isCardFoundInCache = true;
            }
            else
            {
                var reviewCard = studyQueueList.ReviewFlashCard.Find(fc => fc.Id == flashCardId);
                if (reviewCard != null)
                {
                    studyQueueList.ReviewFlashCard.Remove(reviewCard);
                    isCardFoundInCache = true;
                }
            }
        }

        if (currentState != null && currentState.Repetition > 0 &&
            currentState.LastReviewedAt.GetValueOrDefault().Date == DateTime.UtcNow.Date)
        {
            if (isCardFoundInCache && shouldRemoveFromCache)
            {
                var serializedResult = JsonSerializer.Serialize(studyQueueList);
                var ttl = CalculateTtlUntilEndOfDay();

                await _redisService.SetAsync(key, serializedResult);
                await _redisService.SetExpiryAsync(key, ttl);
            }
            return OperationResult.SuccessResult("Thẻ này đã được ôn tập trong hôm nay. Không thực hiện tính toán SRS lại.");
        }

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            double newDifficulty;
            double newStability;

            if (currentState!.Repetition == 0)
            {
                newDifficulty = _srsService.CalculateInitialDifficulty(rating);
                newStability = _srsService.CalculateInitialStability(rating);
            }
            else
            {
                newDifficulty = _srsService.CalculateNewDifficulty(currentState.Difficulty, rating);
                newStability = _srsService.CalculateNewStability(currentState.Stability, newDifficulty, rating);
            }

            int newInterval = _srsService.CalculateOptimalInterval(newStability, card.Deck.DesiredRetention);

            if (rating == RatingLearningCard.Forgotten)
            {
                newInterval = 1;
            }

            currentState.Difficulty = newDifficulty;
            currentState.Stability = newStability;
            currentState.Interval = newInterval;
            currentState.NextReviewDate = DateTime.UtcNow.AddDays(newInterval);
            currentState.Repetition = currentState.Repetition + 1;
            currentState.LastReviewedAt = DateTime.UtcNow;
            currentState.LastRating = rating;

            _unitOfWork.Repository<FlashCardStateEntity>().UpdateAsync(currentState);

            if (!await _unitOfWork.SaveChangesAsync())
                throw new InternalServerException(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(FlashCardEntity)), "");

            await _reviewFLHistoryService.SaveReviewHistorySnapshotAsync(currentState.Id, rating, currentState.Stability, currentState.Difficulty, currentState.Interval, currentState.Repetition);
        });

        if (isCardFoundInCache && shouldRemoveFromCache)
        {
            var serializedResult = JsonSerializer.Serialize(studyQueueList);
            var ttl = CalculateTtlUntilEndOfDay();

            await _redisService.SetAsync(key, serializedResult);
            await _redisService.SetExpiryAsync(key, ttl);
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(FlashCardEntity)), flashCardId);
    }

    public async Task<CursorQueryResult<FlashCardStudyModel>> GetFlashCardsByDeckID(CursorQueryInfo queryInfo, Guid deckId, Guid userId)
    {

        var deckProps = await _deckService.GetDeckPropsAsync(deckId);

        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        var (entities, nextCursor) = await _flashCardsRepository.GetAllCardsForDeckByCursorAsync(
            deckId,
            queryInfo.AfterCursor,
            queryInfo.Limit
        );
        var models = _mapper.Map<List<FlashCardStudyModel>>(entities);

        return new CursorQueryResult<FlashCardStudyModel>
        {
            Data = models,
            NextCursor = nextCursor,
            Total = models.Count
        };
    }
    public async Task<OperationResult> CloneWholeDeckAsync(Guid sourceDeckId, Guid userId)
    {
        if (sourceDeckId == Guid.Empty)
            throw new BadRequestException(ErrorMessageBase.Format(ErrorMessageBase.Required, nameof(sourceDeckId)));

        var sourceDeck = await _unitOfWork.Repository<DeckEntity>().GetByIdAsync(sourceDeckId)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, sourceDeckId, nameof(DeckEntity)));

        if (!sourceDeck.IsPublic)
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        var sourceCards = await _flashCardsRepository.GetAllCardsForDeckAsync(sourceDeckId);

        Guid newDeckId = Guid.Empty;

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            var newDeck = DeckEntity.CloneFrom(sourceDeck, userId);
            await _unitOfWork.Repository<DeckEntity>().AddAsync(newDeck);

            var newCards = new List<FlashCardEntity>();
            var newStates = new List<FlashCardStateEntity>();

            foreach (var card in sourceCards)
            {
                var newCard = FlashCardEntity.CloneFrom(card, newDeck.Id);

                newCards.Add(newCard);

                var newState = FlashCardStateEntity.CreateNew(newCard.Id);
                newStates.Add(newState);
            }

            if (newCards.Count != 0)
            {
                await _unitOfWork.Repository<FlashCardEntity>().AddRangeAsync(newCards);
                await _unitOfWork.Repository<FlashCardStateEntity>().AddRangeAsync(newStates);
            }

            await _unitOfWork.SaveChangesAsync();
            newDeckId = newDeck.Id;
        });

        return OperationResult.SuccessResult(ErrorMessageBase.CreatedSuccess, newDeckId);
    }

    public async Task<OperationResult> ImportCardsFromDeckAsync(Guid sourceDeckId, Guid targetDeckId, Guid userId)
    {

        if (!await _deckService.CheckDeckOwner(targetDeckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Format(ErrorMessageBase.Forbidden, $"Deck Id {targetDeckId}"));

        var sourceDeck = await _unitOfWork.Repository<DeckEntity>().GetByIdAsync(sourceDeckId)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, sourceDeckId, nameof(DeckEntity)));

        if (!sourceDeck.IsPublic)
            throw new ForbiddenException(ErrorMessageBase.Format(ErrorMessageBase.Forbidden, $"Deck Id {sourceDeckId}"));

        var sourceCards = await _flashCardsRepository.GetAllCardsForDeckAsync(sourceDeckId);
        if (sourceCards.Count == 0)
            return OperationResult.SuccessResult(ErrorMessageBase.CreateFailure, "Cards");

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            var newCards = new List<FlashCardEntity>();
            var newStates = new List<FlashCardStateEntity>();

            foreach (var card in sourceCards)
            {
                var newCard = FlashCardEntity.CloneFrom(card, targetDeckId);

                newCards.Add(newCard);

                var newState = FlashCardStateEntity.CreateNew(newCard.Id);
                newStates.Add(_mapper.Map<FlashCardStateEntity>(newState));
            }
            sourceDeck.TotalUsersUsing += 1;

            await _unitOfWork.Repository<FlashCardEntity>().AddRangeAsync(newCards);
            await _unitOfWork.Repository<FlashCardStateEntity>().AddRangeAsync(newStates);
            _unitOfWork.Repository<DeckEntity>().UpdateAsync(sourceDeck);
            await _unitOfWork.SaveChangesAsync();
        });

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, targetDeckId), sourceCards.Count + " Cards");
    }
    public async Task<OperationResult> CopyFlashCardsToDeckAsync(FlashCardsToDeckRequestModel model, Guid targetDeckId, Guid userId)
    {

        if (!await _deckService.CheckDeckOwner(targetDeckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Format(ErrorMessageBase.Forbidden, $"Deck Id {targetDeckId}"));


        var sourceCards = await _flashCardsRepository.GetAllCardsForByIdsAsync(model.FlashCardIds);

        if (sourceCards.Count == 0)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(FlashCardEntity)));


        if (sourceCards.Any(c => c.DeckId != model.DeckSourceId) || !sourceCards[0].Deck.IsPublic || sourceCards[0].Deck.UserCreateId != userId)
            throw new ForbiddenException(ErrorMessageBase.Format(ErrorMessageBase.Forbidden, $"Deck Id {model.DeckSourceId}"));

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            var newCards = new List<FlashCardEntity>();
            var newStates = new List<FlashCardStateEntity>();

            foreach (var card in sourceCards)
            {
                var newCard = FlashCardEntity.CloneFrom(card, targetDeckId);

                newCards.Add(newCard);

                var newState = FlashCardStateEntity.CreateNew(newCard.Id);
                newStates.Add(_mapper.Map<FlashCardStateEntity>(newState));
            }

            await _unitOfWork.Repository<FlashCardEntity>().AddRangeAsync(newCards);
            await _unitOfWork.Repository<FlashCardStateEntity>().AddRangeAsync(newStates);
            await _unitOfWork.SaveChangesAsync();
        });

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, targetDeckId), sourceCards.Count + " Cards");
    }

    public async Task<OperationResult> MoveFlashCardsToDeckAsync(FlashCardsToDeckRequestModel model, Guid targetDeckId, Guid userId)
    {

        if (!await _deckService.CheckDeckOwner(targetDeckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Format(ErrorMessageBase.Forbidden, $"Deck Id {targetDeckId}"));

        var cardsToMove = await _flashCardsRepository.GetAllCardsForByIdsAsync(model.FlashCardIds);

        if (cardsToMove.Count == 0)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(FlashCardEntity)));

        if (cardsToMove.Any(c => c.DeckId != model.DeckSourceId) || cardsToMove[0].Deck.UserCreateId != userId)
            throw new ForbiddenException(ErrorMessageBase.Format(ErrorMessageBase.Forbidden, $"Deck Id {model.DeckSourceId}"));

        foreach (var card in cardsToMove)
        {
            card.DeckId = targetDeckId;

            _unitOfWork.Repository<FlashCardEntity>().UpdateAsync(card);
        }

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, "Cards"));

        return OperationResult.SuccessResult(ErrorMessageBase.UpdatedSuccess, "Cards");
    }

    public async Task<OperationResult> ResetProgressAsync(Guid deckId, Guid userId)
    {
        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        var flashCardStates = await _flashCardStateService.GetFlashCardsStateByDeckId(deckId);

        var updateState = _flashCardStateService.ResetStateCards(flashCardStates);

        if (!updateState.Success || !await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, "Cards"));

        return OperationResult.SuccessResult(ErrorMessageBase.UpdatedSuccess, "Cards");
    }

    public async Task<OperationResult> CreateFromVocabularyAsync(CreateFlashCardByVocabularyModel vocabIds, Guid userId, Guid deckId)
    {
        if (!await _deckService.CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Format(ErrorMessageBase.Forbidden, $"Deck Id {deckId}"));

        var initialVocabIds = vocabIds.VocabularyIds;

        var existingCards = await _flashCardsRepository.GetFlashCardsInDeckByRelationVocabularyId(vocabIds.VocabularyIds, deckId);

        var existingVocabIds = existingCards
        .Select(card => card.RelationVocabularyCardId)
        .ToHashSet();

        var newVocabIds = initialVocabIds
        .Where(id => !existingVocabIds.Contains(id))
        .ToList();

        if (newVocabIds.Count == 0)
        {
            return OperationResult.SuccessResult("All selected vocabularies already have flashcards in this deck.");
        }

        var vocabularies = await _vocabularyService.GetVocabulariesByIdsAsync(newVocabIds);

        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
        {
            var newCards = new List<FlashCardEntity>();
            var newStates = new List<FlashCardStateEntity>();
            foreach (var vocab in vocabularies)
            {
                var newCard = FlashCardEntity.CreateFromVocabulary(vocab, deckId);
                newCards.Add(newCard);
                var newState = FlashCardStateEntity.CreateNew(newCard.Id);
                newStates.Add(newState);
            }
            await _unitOfWork.Repository<FlashCardEntity>().AddRangeAsync(newCards);
            await _unitOfWork.Repository<FlashCardStateEntity>().AddRangeAsync(newStates);
            if (!await _unitOfWork.SaveChangesAsync())
                OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(FlashCardEntity)), "");
        });

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(FlashCardEntity)), newVocabIds.Count);
    }

    public async Task<FlashCardStatisticsModel> GetUserFlashCardStatisticsAsync(Guid userId)
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = DateTime.UtcNow.AddDays(-1);

        var cardQuery = await _flashCardsRepository.GetFlashCardsOfUserHasRelationVocabularyCardId(userId);

        if (cardQuery.Count == 0)
        {
            return new FlashCardStatisticsModel();
        }

        var totalLearned = cardQuery.Count(fcs => fcs.CardState.Repetition > 0);

        var totalDueToday = cardQuery.Count(fcs => fcs.CardState.NextReviewDate.HasValue &&
                                                       fcs.CardState.NextReviewDate.Value.Date <= today);

        var totalScheduledFuture = cardQuery.Count(fcs => fcs.CardState.Repetition > 0 &&
                                                       fcs.CardState.NextReviewDate.HasValue &&
                                                       fcs.CardState.NextReviewDate.Value.Date > today);

        var totalReviewedToday = await reviewFLHistoryService.GetReviewHistoryByUserIdToday(userId);

        return new FlashCardStatisticsModel
        {
            TotalCardsLinkedToVocabulary = cardQuery.Count,
            TotalLearnedCards = totalLearned,
            TotalCardsDueToday = totalDueToday,
            TotalCardsScheduledForFutureReview = totalScheduledFuture,
            TotalCardsReviewedToday = totalReviewedToday
        };
    }
    public async Task<Dictionary<Guid, int>> GetNumberNeedReviewTodayForUserIds(List<Guid> userIds)
    {
        return await _flashCardsRepository.GetNumberNeedReviewTodayForUserIds(userIds);
    }

    ///=========================Private function==================================

    private async Task<string?> UploadFileIfPresentAsync(IFormFile? file, List<string> uploadedUrls)
    {
        if (file == null || file.Length == 0) return string.Empty;

        var url = await _blobStorageService.UploadFileAsync(AppConstants.BlobContainerFlashCard, file);

        if (!string.IsNullOrEmpty(url))
        {
            uploadedUrls.Add(url);
        }
        return url;
    }

    private static string? GetOldFileUrl(string contentJson, string type)
    {
        try
        {
            var contents = JsonSerializer.Deserialize<List<FlashCardContentsModel>>(contentJson);
            return contents?.FirstOrDefault(c => c.Type!.Equals(type, StringComparison.OrdinalIgnoreCase))?.Text;
        }
        catch (Exception ex)
        {
            throw new InternalServerException(ErrorMessageBase.UpdateFailure, ex.Message);
        }
    }

    private async Task DeleteOldFileIfReplacedAsync(string? newUrl, string? oldUrl)
    {
        if (!string.IsNullOrEmpty(newUrl) && !string.IsNullOrEmpty(oldUrl) && newUrl != oldUrl)
        {
            try
            {
                await _blobStorageService.DeleteFileAsync(AppConstants.BlobContainerFlashCard, oldUrl);
            }
            catch (Exception)
            {

            }
        }
    }
    private async Task CleanUpUploadedFilesAsync(List<string> uploadedUrls)
    {
        foreach (var url in uploadedUrls)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    await _blobStorageService.DeleteFileAsync(AppConstants.BlobContainerFlashCard, url);
                }
                catch (Exception)
                {

                }
            }
        }
    }
    public static string CreateContentJson(string? text, string? urlImg, string? urlAudio)
    {

        var flashCardContents = new List<FlashCardContentsModel>();

        if (!string.IsNullOrEmpty(text))
        {
            flashCardContents.Add(new FlashCardContentsModel
            {
                Type = "Text",
                Text = text
            });
        }

        if (!string.IsNullOrEmpty(urlImg))
        {
            flashCardContents.Add(new FlashCardContentsModel
            {
                Type = "Image",
                Text = urlImg
            });
        }

        if (!string.IsNullOrEmpty(urlAudio))
        {
            flashCardContents.Add(new FlashCardContentsModel
            {
                Type = "Audio",
                Text = urlAudio
            });
        }

        if (!flashCardContents.Any())
        {
            return "[]";
        }

        return JsonSerializer.Serialize(flashCardContents);
    }

    private static List<string> ExtractAllUrls(FlashCardEntity entity)
    {
        var urls = new List<string>();

        var frontImg = GetOldFileUrl(entity.FrontContent, "Image");
        var frontAudio = GetOldFileUrl(entity.FrontContent, "Audio");
        var backImg = GetOldFileUrl(entity.BackContent, "Image");
        var backAudio = GetOldFileUrl(entity.BackContent, "Audio");

        if (!string.IsNullOrEmpty(frontImg)) urls.Add(frontImg);
        if (!string.IsNullOrEmpty(frontAudio)) urls.Add(frontAudio);
        if (!string.IsNullOrEmpty(backImg)) urls.Add(backImg);
        if (!string.IsNullOrEmpty(backAudio)) urls.Add(backAudio);

        return urls.Distinct().ToList();
    }

    private static TimeSpan CalculateTtlUntilEndOfDay()
    {
        var currentTime = DateTimeOffset.UtcNow;

        var tomorrowStartOfDayUtc = currentTime.Date.AddDays(1);

        var expirationTime = new DateTimeOffset(tomorrowStartOfDayUtc, TimeSpan.Zero);

        var ttl = expirationTime - currentTime;

        if (ttl.TotalSeconds <= 0)
        {

            return TimeSpan.FromSeconds(1);
        }
        return ttl;
    }
}
