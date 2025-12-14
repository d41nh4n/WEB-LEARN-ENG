namespace Allen.Infrastructure;

public interface IDeckRepository : IRepositoryBase<DeckEntity>
{
    Task<DeckModel> GetDeckModelById(Guid deckId);
    Task<List<DeckModel>> GetDeckModelByUserId(Guid userId);
    Task<bool> CheckDeckOwnerAsync(Guid deckId, Guid userId);
    Task<DeckPropsModel> GetDeckPropsAsync(Guid deckId);
}