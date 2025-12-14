namespace Allen.Infrastructure;

public interface IFlashCardsRepository : IRepositoryBase<FlashCardEntity>
{
    Task<QueryResult<FlashCardModel>> GetFlashCardsAsync(QueryInfo queryInfo);
    Task<FlashCardEntity> GetFlashCardByIdAsync(Guid id);
    Task<QueryResult<Guid>> GetFlashCardIdsByDeck(QueryInfo queryInfo, Guid deckId);

    Task<List<Guid>> GetListUserIdToNotificateStudyToday();
    Task<Dictionary<Guid, int>> GetNumberNeedReviewTodayForUserIds(List<Guid> userIds);
    //------------------------Study Card-----------------------
    Task<List<FlashCardEntity>> GetSrsStudyQueueAsync(Guid deckId, DateTime now, int newCardLimit);
    Task<(List<FlashCardEntity> Data, long? NextCursor)> GetAllCardsForDeckByCursorAsync(Guid deckId, long? afterCursor, int limit);

    Task<List<FlashCardEntity>>  GetAllCardsForDeckAsync(Guid deckId);
    Task<List<FlashCardEntity>> GetAllCardsForByIdsAsync(List<Guid> flashCardIds);

    Task<List<FlashCardEntity>> GetFlashCardsInDeckByRelationVocabularyId(List<Guid> flashCardIds, Guid deckId);

    Task<List<FlashCardEntity>> GetAllCardsForByIdsAndDeckIdAsync(List<Guid> flashCardIds, Guid deckId);
    Task<List<FlashCardEntity>> GetFlashCardsOfUserHasRelationVocabularyCardId(Guid userId);
}