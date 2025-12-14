namespace Allen.Infrastructure;

[RegisterService(typeof(IReactionsRepository))]
public class ReactionRepository(
    SqlApplicationDbContext context
) : RepositoryBase<ReactionEntity>(context), IReactionsRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<List<Reaction>> GetReactionsAsync(Guid objectId, ObjectType objectType)
    {
        return await _context.Reactions
            .Where(r => r.ObjectId == objectId && r.ObjectType == objectType)
            .Select(r => new Reaction
            {
                Id = r.Id,
                ObjectId = r.ObjectId,
                ObjectType = r.ObjectType.ToString(),
                UserId = r.UserId,
                UserName = r.User.Name,
                UserPicture = r.User.Picture,
                ReactionType = r.ReactionType.ToString()
            })
            .ToListAsync();

    }

    public async Task<Reaction?> GetReactionByUserAsync(Guid userId, Guid objectId, ObjectType objectType)
    {
        return await _context.Reactions
            .Where(r => r.UserId == userId && r.ObjectId == objectId && r.ObjectType == objectType)
            .Select(r => new Reaction
            {
                Id = r.Id,
                ObjectId = r.ObjectId,
                ObjectType = r.ObjectType.ToString(),
                UserId = r.UserId,
                UserName = r.User.Name,
                UserPicture = r.User.Picture,
                ReactionType = r.ReactionType.ToString()
            })
        .FirstOrDefaultAsync();
    }

    public async Task<List<ReactionUserModel>> GetUsersByReactionAsync(Guid objectId, ObjectType objectType, ReactionType reactionType)
    {
        return await _context.Reactions
            .Where(r => r.ObjectId == objectId
                     && r.ObjectType == objectType
                     && r.ReactionType == reactionType)
            .Select(r => new ReactionUserModel
            {
                UserId = r.UserId,
                UserName = r.User.Name,
                UserPicture = r.User.Picture
            })
        .ToListAsync();
    }

    public async Task<List<ReactionSummaryModel>> GetSummaryReactionAsync(Guid objectId, ObjectType objectType)
    {
        return await _context.Reactions
            .Where(r => r.ObjectId == objectId && r.ObjectType == objectType)
            .GroupBy(r => r.ReactionType)
            .Select(g => new ReactionSummaryModel
            {
                ReactionType = g.Key.ToString(),
                Count = g.Count(),
                Users = g.Select(r => new ReactionUserModel
                {
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    UserPicture = r.User.Picture
                }).ToList()
            })
            .ToListAsync();
    }
}
