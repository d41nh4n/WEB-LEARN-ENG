namespace Allen.Application;

public interface IReactionService
{
    Task<IEnumerable<Reaction>> GetReactionsAsync(Guid objectId);
    Task<Reaction?> GetReactionByUserAsync(Guid userId, Guid objectId);
    Task<IEnumerable<ReactionUserModel>> GetUsersByReactionAsync(Guid objectId, string reactionType);
    Task<OperationResult> CreateOrUpdateReactionAsync(CreateOrUpdateReactionModel model);
    Task<IEnumerable<ReactionSummaryModel>> GetSummaryReactionAsync(Guid objectId);
}
