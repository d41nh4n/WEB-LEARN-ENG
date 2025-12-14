namespace Allen.Infrastructure;

[RegisterService(typeof(ITopicRepository))]
public class TopicRepository(SqlApplicationDbContext context) : RepositoryBase<TopicEntity>(context), ITopicRepository
{
	private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<TopicModel?>> GetTopicsAsync(QueryInfo queryInfo)
    {
        var query = _context.Topics.AsNoTracking();
        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new TopicModel
            {
                Id = x.Id,
                TopicName = x.TopicName ?? "",
                TopicDecription = x.TopicDecription ?? "",
            })
            .ToListAsync();

        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await query.CountAsync();
        }
        return new QueryResult<TopicModel?>
        {
            Data = entities,
            TotalCount = totalItems
        };

    }
    public async Task<TopicModel?> GetTopicByIdAsync(Guid id)
    {
        return await _context.Topics.AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new TopicModel
        {
            Id = x.Id,
            TopicName = x.TopicName ?? "",
            TopicDecription = x.TopicDecription ?? "",
        })
        .FirstOrDefaultAsync();
    }
}
