namespace Allen.Infrastructure;

[RegisterService(typeof(ICommentsRepository))]
public class CommentsRepository(
    SqlApplicationDbContext context
) : RepositoryBase<CommentEntity>(context), ICommentsRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<Comment>> GetRootCommentsAsync(Guid objectId, QueryInfo queryInfo)
    {
        var replyCounts = await _context.Comments
            .Where(c => c.Id != c.CommentParentId)
            .GroupBy(c => c.CommentParentId)
            .Select(g => new { CommentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CommentId, g => g.Count);

        var reactionCounts = await _context.Reactions
            .Where(r => r.ObjectType == ObjectType.Comment)
            .GroupBy(r => r.ObjectId)
            .Select(g => new { CommentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CommentId, g => g.Count);

        var baseQuery = _context.Comments
            .AsNoTracking()
            .Where(c => c.ObjectId == objectId && c.Id == c.CommentParentId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(c => new
            {
                Root = c,
                User = c.User
            });

        var data = await baseQuery.ToListAsync();

        var result = data.Select(x => new Comment
        {
            Id = x.Root.Id,
            ObjectId = x.Root.ObjectId,
            UserId = x.Root.UserId,
            UserAvatar = x.User?.Picture,
            UserName = x.User?.Name,
            Content = x.Root.Content,
            CommentParentId = x.Root.CommentParentId,
            TotalReaction = reactionCounts.GetValueOrDefault(x.Root.Id),
            ReplyCount = replyCounts.GetValueOrDefault(x.Root.Id),
            CreatedAt = x.Root.CreatedAt
        }).ToList();

        var total = queryInfo.NeedTotalCount
            ? await _context.Comments.CountAsync(c => c.ObjectId == objectId && c.Id == c.CommentParentId)
            : 0;

        return new QueryResult<Comment>
        {
            Data = result,
            TotalCount = total
        };
    }

    public async Task<QueryResult<Comment>> GetRepliesAsync(Guid parentId, QueryInfo queryInfo)
    {
        var reactionCounts = await _context.Reactions
            .Where(r => r.ObjectType == ObjectType.Comment)
            .GroupBy(r => r.ObjectId)
            .Select(g => new { CommentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CommentId, g => g.Count);

        var replyCounts = await _context.Comments
            .Where(c => c.Parent != null)
            .GroupBy(c => c.Parent != null ? c.Parent.Id : Guid.Empty)
            .Select(g => new { ParentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.ParentId, g => g.Count);

        var replies = await _context.Comments
            .AsNoTracking()
            .Where(c => EF.Property<Guid?>(c, "ParentId") == parentId)
            .OrderBy(c => c.CreatedAt)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(c => new
            {
                Reply = c,
                ReplyUser = c.User,
                ParentUser = c.Parent != null ? c.Parent.User : null,
                ParentId = EF.Property<Guid?>(c, "ParentId")
            })
            .ToListAsync();

        var data = replies.Select(x => new Comment
        {
            Id = x.Reply.Id,
            ObjectId = x.Reply.ObjectId,
            UserId = x.Reply.UserId,
            UserAvatar = x.ReplyUser?.Picture,
            UserName = x.ReplyUser?.Name,
            Content = x.Reply.Content,
            CommentParentId = x.Reply.CommentParentId,
            ParentId = x.ParentId,
            ReplyToUserName = x.ParentUser?.Name,
            TotalReaction = reactionCounts.GetValueOrDefault(x.Reply.Id),
            CreatedAt = x.Reply.CreatedAt,
            ReplyCount = replyCounts.GetValueOrDefault(x.Reply.Id, 0)
        }).ToList();

        var total = queryInfo.NeedTotalCount
            ? await _context.Comments.CountAsync(c => c.Parent != null && c.Parent.Id == parentId)
            : 0;

        return new QueryResult<Comment>
        {
            Data = data,
            TotalCount = total
        };
    }

    public async Task<List<CommentEntity>> GetCommentsByParentIdAsync(Guid parentId)
    {
        return await _context.Comments
            .Where(c => c.CommentParentId == parentId)
            .ToListAsync();
    }

    public async Task ReassignRepliesParentAsync(CommentEntity comment, CommentEntity newParent)
    {
        var replies = await _context.Comments
            .Where(c => c.Parent != null && c.Parent.Id == comment.Id)
            .ToListAsync();

        foreach (var reply in replies)
        {
            reply.Parent = newParent;
            reply.CommentParentId = newParent.CommentParentId;
        }
    }

    public async Task<CommentEntity> GetCommentByIdAsync(Guid id)
    {
        var entity = await _context.Comments
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity ?? throw new NotFoundException($"Comment with {id} was not found.");
    }
}
