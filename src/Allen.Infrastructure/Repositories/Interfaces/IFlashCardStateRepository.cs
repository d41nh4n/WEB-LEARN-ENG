namespace Allen.Infrastructure;
public interface IFlashCardStateRepository
{
    Task DeleteByFlashCardIdAsync(Guid flashCardId);
    Task<List<FlashCardStateEntity>> GetFlashCardsStateByDeckIdAsync(Guid deckID);
}
