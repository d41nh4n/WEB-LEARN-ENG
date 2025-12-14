namespace Allen.Application;

public interface IDeckService
{
    Task<DeckModel> GetByIdAsync(Guid id);
    Task<List<DeckModel>> GetByUserIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateDeckModel model, Guid userId);
    Task<OperationResult> DeleteByIdAsync(Guid userId, Guid deckId);
    Task<OperationResult> UpdateAsync(UpdateDeckModel model, Guid deckId, Guid userId);
    Task<bool> CheckDeckOwner(Guid deckId, Guid userId);
    Task<DeckPropsModel> GetDeckPropsAsync(Guid deckId);
}
