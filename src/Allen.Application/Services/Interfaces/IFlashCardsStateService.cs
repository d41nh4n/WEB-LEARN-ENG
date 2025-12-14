namespace Allen.Application;
    public interface IFlashCardsStateService
{
    Task<FlashCardStateEntity> CreateAsync(FlashCardStateModel model);
    Task<OperationResult> DeleteAsync(Guid id);
    Task<QueryResult<FlashCardStateModel>> GetByFlashCardIdAsync(Guid flashCardId);
    OperationResult ResetStateCards(List<FlashCardStateEntity> flashCardState);
    Task<List<FlashCardStateEntity>> GetFlashCardsStateByDeckId(Guid flashCardId);
}
