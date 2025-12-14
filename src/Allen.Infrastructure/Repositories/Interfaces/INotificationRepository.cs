namespace Allen.Infrastructure;

public interface INotificationRepository : IRepositoryBase<NotificationEntity>
{
    Task<QueryResult<NotificationEntity>> GetUserNotificationsAsync(Guid userId, QueryInfo queryInfo, NotificationQuery query);
}