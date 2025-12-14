namespace Allen.Infrastructure;

public interface IReactionsRepository : IRepositoryBase<ReactionEntity>
{
    Task<List<Reaction>> GetReactionsAsync(Guid objectId, ObjectType objectType);
    Task<Reaction?> GetReactionByUserAsync(Guid userId, Guid objectId, ObjectType objectType);
    Task<List<ReactionUserModel>> GetUsersByReactionAsync(Guid objectId, ObjectType objectType, ReactionType reactionType);
    Task<List<ReactionSummaryModel>> GetSummaryReactionAsync(Guid objectId, ObjectType objectType);
}
