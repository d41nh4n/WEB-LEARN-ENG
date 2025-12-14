namespace Allen.Infrastructure;

[RegisterService(typeof(ITagRepository))]

public class TagRepository(SqlApplicationDbContext context
) : RepositoryBase<TagEntity>(context), ITagRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<int> CountExistingTagsAsync(List<Guid> tagsId)
    {
        return await _context.Tags.CountAsync(t => tagsId.Contains(t.Id));
    }

    public async Task<IEnumerable<Guid>> GetAllTagsAsync(List<Guid> tagsId)
    {
        return await _context.Tags
        .Where(t => tagsId.Contains(t.Id))
        .Select(t => t.Id)
        .ToListAsync();
    }

    public async Task<string?> GetNameTagByIdsAsync(Guid id)
    {
        var nameTag = await _context.Tags.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => x.NameTag)
            .FirstOrDefaultAsync();

        return nameTag;
    }

    public async Task<QueryResult<TagModel?>> GetTagsAsync(QueryInfo queryInfo)
    {
        var query = _context.Tags.AsNoTracking();
        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new TagModel
            {
                Id = x.Id,
                NameTag = x.NameTag ?? "",
            })
            .ToListAsync();

        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await query.CountAsync();
        }
        return new QueryResult<TagModel?>
        {
            Data = entities,
            TotalCount = totalItems
        };
    }
}
