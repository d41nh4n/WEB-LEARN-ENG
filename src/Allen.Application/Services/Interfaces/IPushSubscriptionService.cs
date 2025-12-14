namespace Allen.Application;

public interface IPushSubscriptionService
{
    Task<OperationResult> SubscribeAsync(Guid userId, SubscriptionModel model);
    Task<OperationResult> UnsubscribeAsync(string endpoint, Guid userId);
    Task<OperationResult> SendNotificationAsync(Guid userId, PushNotificationPayload notificationPayload);
    Task SendNotificationToStudyFlashCardTodayAsync();
}
