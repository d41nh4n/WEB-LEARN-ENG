namespace Allen.API.Controllers;

[Route("notifications")]
[Authorize]
public class NotificationsController(
    IEmailService _emailService, 
    INotificationService _notificationService,
    IPushSubscriptionService _pushSubscriptionService) : BaseApiController
{
    [HttpPost("send-email")]
    [AllowAnonymous]
	public IActionResult SendEmail(EmailContent emailContent)
    {
        _emailService.SendMailAsync(emailContent);
        return Ok(new { message = "Email sent successfully." });
    }

    [HttpGet("user")]
    public async Task<QueryResult<NotificationEntity>> GetUserNotifications([FromQuery] QueryInfo queryInfo, [FromQuery] NotificationQuery query)
    {
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _notificationService.GetUserNotificationsAsync(userId, queryInfo, query);
    }

    [HttpPatch("{notificationId}/mark-read")]
    public async Task<OperationResult> MarkAsRead(Guid notificationId)
    {
        return await _notificationService.MarkAsReadAsync(notificationId);
    }

    [HttpPatch("user/mark-all-read")]
    public async Task<OperationResult> MarkAllAsRead()
    {
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _notificationService.MarkAllAsReadAsync(userId);
    }

    [HttpDelete("{notificationId}")]
    public async Task<OperationResult> Delete(Guid notificationId)
    {
        return await _notificationService.DeleteAsync(notificationId);
    }

    //=========================== Push Subscription ===========================
    [HttpPost("push-subcription")]
    [Authorize]
    public async Task<OperationResult> SubcribeNotification([FromBody]SubscriptionModel model)
    {
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);

       return await _pushSubscriptionService.SubscribeAsync(userId, model);
    }

    [HttpPost("push-unsubcription")]
    [Authorize]
    public async Task<OperationResult> UnsubcribeNotification([FromBody] UnSubcriptionModel endpoint)
    {
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);

        return await _pushSubscriptionService.UnsubscribeAsync(endpoint.Endpoint, userId);
    }

    [HttpPost("send-notification/{id}")]
    [AllowAnonymous]
    public async Task<OperationResult> SendNotificationAsync(Guid userId, [FromBody] PushNotificationPayload notificationPayload)
    {
        //var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        //await _pushSubscriptionService.SendNotificationToStudyFlashCardTodayAsync();
        return await _pushSubscriptionService.SendNotificationAsync(userId, notificationPayload);
    }
}