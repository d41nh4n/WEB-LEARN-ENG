namespace Allen.Infrastructure;

[RegisterService(typeof(IPostsRepository))]
public class PostsRepository(
    SqlApplicationDbContext context
) : RepositoryBase<PostEntity>(context), IPostsRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<Post>> GetMyPostsAsync(GetPostQuery query, QueryInfo queryInfo)
    {
        var postQuery = _context.Posts.Where(p => p.UserId == query.UserId);

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            postQuery = postQuery.Where(p =>
                EF.Functions.Collate(p.Content, "Latin1_General_CI_AI")
                    .Contains(queryInfo.SearchText));
        }

        var totalCount = queryInfo.NeedTotalCount ? await postQuery.CountAsync() : 0;

        var postIds = await postQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(p => p.Id)
            .ToListAsync();

        var reactionCounts = await _context.Reactions
            .Where(r => r.ObjectType == ObjectType.Post && postIds.Contains(r.ObjectId))
            .GroupBy(r => r.ObjectId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count);

        var commentCounts = await _context.Comments
            .Where(c => c.ObjectType == ObjectType.Post && postIds.Contains(c.ObjectId))
            .GroupBy(c => c.ObjectId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count);

        var posts = await _context.Posts
            .AsNoTracking()
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new { p, p.User })
            .ToListAsync();

        var data = posts.Select(x =>
        {
            reactionCounts.TryGetValue(x.p.Id, out var rc);
            commentCounts.TryGetValue(x.p.Id, out var cc);

            return new Post
            {
                Id = x.p.Id,
                UserId = x.p.UserId,
                UserAvatar = x.User?.Picture,
                UserName = x.User?.Name,
                Content = x.p.Content,
                Medias = StringHelper.ParseStringToList(x.p.Medias),
                Privacy = x.p.Privacy.ToString(),
                TotalReaction = rc,
                TotalComment = cc,
                CreatedAt = x.p.CreatedAt?.ToString("dd/MM/yyyy HH:mm")
            };
        }).ToList();

        return new QueryResult<Post>
        {
            Data = data,
            TotalCount = totalCount
        };
    }

    public async Task<QueryResult<Post>> GetPostsWithPagingAsync(GetPostQuery postQuery, QueryInfo queryInfo)
    {
        if (!Enum.TryParse(postQuery.Privacy, true, out PrivacyType privacyEnum))
        {
            throw new ArgumentException("Invalid privacy type");
        }

        var blockedUserIds = await _context.UserBlocks
            .Where(b => b.BlockedByUserId == postQuery.UserId)
            .Select(b => b.BlockedUserId)
            .ToListAsync();

        var postIdsQuery = _context.Posts
            .Where(p => p.Privacy == privacyEnum &&
                        !blockedUserIds.Contains(p.UserId) &&
                        (queryInfo.SearchText == null ||
                         EF.Functions.Collate(p.Content, "Latin1_General_CI_AI").Contains(queryInfo.SearchText)))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => p.Id);

        var total = queryInfo.NeedTotalCount ? await postIdsQuery.CountAsync() : 0;

        var postIds = await postIdsQuery
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var reactionCounts = await _context.Reactions
            .Where(r => r.ObjectType == ObjectType.Post && postIds.Contains(r.ObjectId))
            .GroupBy(r => r.ObjectId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count);

        var commentCounts = await _context.Comments
            .Where(c => c.ObjectType == ObjectType.Post && postIds.Contains(c.ObjectId))
            .GroupBy(c => c.ObjectId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count);

        var posts = await _context.Posts
            .AsNoTracking()
            .Where(p => postIds.Contains(p.Id))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new { p, p.User })
            .ToListAsync();

        var data = posts.Select(x =>
        {
            reactionCounts.TryGetValue(x.p.Id, out var rc);
            commentCounts.TryGetValue(x.p.Id, out var cc);

            return new Post
            {
                Id = x.p.Id,
                UserId = x.p.UserId,
                UserAvatar = x.User?.Picture,
                UserName = x.User?.Name,
                Content = x.p.Content,
                Medias = StringHelper.ParseStringToList(x.p.Medias),
                Privacy = x.p.Privacy.ToString(),
                TotalReaction = rc,
                TotalComment = cc,
                CreatedAt = FormatDate(x.p.CreatedAt)
            };
        }).ToList();

        return new QueryResult<Post>
        {
            Data = data,
            TotalCount = total
        };
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        var postWithUser = await _context.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new { p, p.User })
            .FirstOrDefaultAsync();

        if (postWithUser == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Post), id));

        var totalReaction = await _context.Reactions.CountAsync(r =>
            r.ObjectType == ObjectType.Post && r.ObjectId == id);

        var totalComment = await _context.Comments.CountAsync(c =>
            c.ObjectType == ObjectType.Post && c.ObjectId == id);

        return new Post
        {
            Id = postWithUser.p.Id,
            UserId = postWithUser.p.UserId,
            UserAvatar = postWithUser.User?.Picture,
            UserName = postWithUser.User?.Name,
            Content = postWithUser.p.Content,
            Medias = StringHelper.ParseStringToList(postWithUser.p.Medias),
            Privacy = postWithUser.p.Privacy.ToString(),
            TotalReaction = totalReaction,
            TotalComment = totalComment,
            CreatedAt = FormatDate(postWithUser.p.CreatedAt)
        };
    }

    private static string? FormatDate(DateTime? dateTime)
    {
        return dateTime?.ToString("dd/MM/yyyy HH:mm");
    }
}
