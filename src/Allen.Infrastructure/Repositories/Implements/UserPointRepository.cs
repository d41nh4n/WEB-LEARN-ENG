namespace Allen.Infrastructure;

[RegisterService(typeof(IUserPointRepository))]
public class UserPointRepository(SqlApplicationDbContext context) : RepositoryBase<UserPointEntity>(context), IUserPointRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<UserPoint>> GetAllUserPointsAsync(QueryInfo queryInfo)
    {
        var query = _context.UserPoints
            .AsNoTracking()
            .Where(u => queryInfo.SearchText == null ||
                        EF.Functions.Collate(u.User!.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText))
            .OrderByDescending(u => u.TotalPoints)
            .Select(p => new UserPoint
            {
                UserId = p.UserId,
                UserName = p.User != null ? p.User.Name : null,
                TotalPoints = p.TotalPoints
            });

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
        var data = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<UserPoint>
        {
            Data = data,
            TotalCount = total
        };
    }

    public async Task<UserPoint?> GetUserPointByUserIdAsync(Guid userId)
    {
        return await _context.UserPoints
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new UserPoint
            {
                UserId = p.UserId,
                UserName = p.User != null ? p.User.Name : null,
                TotalPoints = p.TotalPoints
            })
            .FirstOrDefaultAsync();
    }
}
