namespace Allen.Infrastructure;

[RegisterService(typeof(INotificationRepository))]
public class NotificationRepository(SqlApplicationDbContext context) : RepositoryBase<NotificationEntity>(context), INotificationRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<NotificationEntity>> GetUserNotificationsAsync(Guid userId, QueryInfo queryInfo, NotificationQuery query)
    {
        var q = _context.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId);

        if (query?.IsRead.HasValue == true)
            q = q.Where(n => n.IsRead == query.IsRead.Value);

        if (!string.IsNullOrWhiteSpace(query?.EventType))
            q = q.Where(n => n.EventType == query.EventType);

        if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
        {
            var search = queryInfo.SearchText.Trim();
            q = q.Where(n =>
                EF.Functions.Collate(n.Title, "Latin1_General_CI_AI").Contains(search) ||
                EF.Functions.Collate(n.Message, "Latin1_General_CI_AI").Contains(search));
        }

        var total = queryInfo.NeedTotalCount ? await q.CountAsync() : 0;

        var ordered = string.IsNullOrEmpty(queryInfo.OrderBy)
            ? q.OrderByDescending(n => n.CreatedAt)
            : queryInfo.OrderType == OrderType.Ascending
                ? q.OrderBy(e => EF.Property<object>(e, queryInfo.OrderBy))
                : q.OrderByDescending(e => EF.Property<object>(e, queryInfo.OrderBy));

        var items = await ordered.Skip(queryInfo.Skip).Take(queryInfo.Top).ToListAsync();

        return new QueryResult<NotificationEntity>
        {
            Data = items,
            TotalCount = total
        };
    }
}