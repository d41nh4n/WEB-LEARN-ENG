namespace Allen.Application;

public interface INotificationService
{
    Task NotifyAsync(NotificationModel model);
    Task<QueryResult<NotificationEntity>> GetUserNotificationsAsync(Guid userId, QueryInfo queryInfo, NotificationQuery query);
    Task<OperationResult> MarkAsReadAsync(Guid notificationId);
    Task<OperationResult> MarkAllAsReadAsync(Guid userId);
    Task<OperationResult> DeleteAsync(Guid notificationId);
}