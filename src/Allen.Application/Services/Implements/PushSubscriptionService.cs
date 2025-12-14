using System.Net;
using WebPush;

namespace Allen.Application;

[RegisterService(typeof(IPushSubscriptionService))]
public class PushSubscriptionService(
    IFlashCardsService _flashCardsServiced,
    ILogger<PushSubscriptionService> _logger,
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IConfiguration _configuration,
    IServiceScopeFactory _scopeFactory) : IPushSubscriptionService
{
    public async Task<OperationResult> SendNotificationAsync(Guid userId, PushNotificationPayload fullPayload)
    {
        var vapidPublicKey = _configuration["ValidSubcriptionKey:PublicKey"];
        var vapidPrivateKey = _configuration["ValidSubcriptionKey:PrivateKey"];
        var vapidSubject = _configuration["ValidSubcriptionKey:Subject"];

        if (string.IsNullOrEmpty(vapidPublicKey) || string.IsNullOrEmpty(vapidPrivateKey))
        {
            return OperationResult.Failure("VAPID keys are not configured. Cannot send push notification");
        }

        var subscriptionEntities = await _unitOfWork.Repository<PushSubscriptionEntity>().GetListByConditionAsync(noti => noti.UserId == userId);

        if (subscriptionEntities == null || subscriptionEntities.Count == 0)
        {
            return OperationResult.Failure("No active Push Subscriptions found for this user.");
        }

        var vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
        var webPushClient = new WebPushClient();

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var payload = JsonSerializer.Serialize(fullPayload, serializerOptions);

        Dictionary<string, object> options = new Dictionary<string, object>
        {
            {
                "headers", new Dictionary<string, object>
                {
                    { "Urgency", fullPayload.Priority }
                }
            },
            { "vapidDetails", vapidDetails },
            { "TTL", fullPayload.TTLInSeconds }
        };

        var sendTasks = new List<Task>();

        int successfulSends = 0;
        int subscriptionsDeleted = 0;

        foreach (var sub in subscriptionEntities)
        {
            var task = SendSingleNotificationAsync(webPushClient, sub, payload, options)
                .ContinueWith(async t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        successfulSends++;
                    }
                    else if (t.IsFaulted)
                    {
                        var ex = t.Exception.InnerException;

                        if (ex is WebPushException webPushEx)
                        {
                            if (webPushEx.StatusCode == HttpStatusCode.NotFound || webPushEx.StatusCode == HttpStatusCode.Gone)
                            {

                                await _unitOfWork.Repository<PushSubscriptionEntity>().DeleteByIdAsync(sub.Id);
                                subscriptionsDeleted++;
                            }
                        }
                        else
                        {
                        }
                    }
                });

            sendTasks.Add(task);
        }

        await Task.WhenAll(sendTasks);

        if (subscriptionsDeleted > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        if (successfulSends > 0)
        {
            string removalInfo = subscriptionsDeleted > 0 ? $" ({subscriptionsDeleted} expired subscriptions removed)." : "";
            return OperationResult.SuccessResult($"Notification sent successfully to {successfulSends} device(s){removalInfo}");
        }
        else if (subscriptionEntities.Count > 0 && subscriptionsDeleted == subscriptionEntities.Count)
        {
            return OperationResult.Failure("All registered subscriptions were expired and have been removed from the database.");
        }
        else
        {
            return OperationResult.Failure($"Failed to send notification to any device. Check logs for details. Total attempts: {subscriptionEntities.Count}");
        }
    }
    private async Task SendSingleNotificationAsync(WebPushClient client, PushSubscriptionEntity sub, string payload, Dictionary<string, object> options)
    {
        var pushSubscription = new WebPush.PushSubscription(
            sub.Endpoint,
            sub.P256DH,
            sub.Auth
        );

        await client.SendNotificationAsync(pushSubscription, payload, options);
    }

    public async Task SendNotificationToStudyFlashCardTodayAsync()
    {
        var userIds = await _flashCardsServiced.GetListUserIdToNotificateStudyToday();

        if (userIds == null || userIds.Count == 0)
        {
            return;
        }

        var cardsDueByUserId = await _flashCardsServiced
            .GetNumberNeedReviewTodayForUserIds(userIds);

        const string targetUrl = "/learning/vocabulary/review";

        var sendTasks = new List<Task>();

        foreach (var userId in userIds)
        {
            cardsDueByUserId.TryGetValue(userId, out int cardsDue);

            string body;

            if (cardsDue > 0)
            {
                body = $"Bạn có {cardsDue} thẻ FlashCard cần xem lại hôm nay! Đừng bỏ lỡ nhé.";
            }
            else
            {
                body = "Bạn có thẻ đến hạn xem lại hôm nay! Bắt đầu học ngay nhoooo (>.<)";
            }

            var fullPayload = new PushNotificationPayload
            {
                Notification = new NotificationOptions
                {
                    Title = "Nhắc nhở Học tập",
                    Body = body,
                    Icon = "/icons/default-bell.png",
                    Data = new NotificationData { Url = targetUrl }
                },
                TTLInSeconds = calculateTTLInSeconds(),
                Priority = "high"
            };

            sendTasks.Add(Task.Run(async () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var pushService = scope.ServiceProvider.GetRequiredService<IPushSubscriptionService>();

                    try
                    {
                        await pushService.SendNotificationAsync(userId, fullPayload);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to send push notification to user {userId}", ex.Message);
                    }
                }
            }));
        }

        await Task.WhenAll(sendTasks);
    }

    public async Task<OperationResult> SubscribeAsync(Guid userId, SubscriptionModel model)
    {
        var pushSubscriptionEntity = _mapper.Map<PushSubscriptionEntity>(model);
        pushSubscriptionEntity.UserId = userId;

        var existingSubscription = await _unitOfWork.Repository<PushSubscriptionEntity>()
            .GetByConditionAsync(noti => noti.Endpoint == pushSubscriptionEntity.Endpoint && noti.UserId == userId);

        if (existingSubscription != null)
            return OperationResult.SuccessResult("Push Subscription already exists for this user.");

        await _unitOfWork.Repository<PushSubscriptionEntity>().AddAsync(pushSubscriptionEntity);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(PushSubscriptionEntity)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(PushSubscriptionEntity)));
    }

    public async Task<OperationResult> UnsubscribeAsync(string endpoint, Guid userId)
    {
        var subscriptionEntity = await _unitOfWork.Repository<PushSubscriptionEntity>().GetByConditionAsync(noti => noti.Endpoint == endpoint);

        if (subscriptionEntity == null)
        {
            return OperationResult.Failure("Push Subscription not found for this user");
        }

        if (subscriptionEntity.UserId != userId)
        {
            throw new ForbiddenException(ErrorMessageBase.Forbidden);
        }

        await _unitOfWork.Repository<PushSubscriptionEntity>().DeleteByIdAsync(subscriptionEntity.Id);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(PushSubscriptionEntity)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(PushSubscriptionEntity)));
    }

    private int calculateTTLInSeconds()
    {
        var currentTime = DateTimeOffset.UtcNow;

        var endOfDay = currentTime.Date.AddDays(1).AddSeconds(-10);

        var expirationTime = new DateTimeOffset(endOfDay, TimeSpan.Zero);

        var ttl = expirationTime - currentTime;

        if (ttl.TotalSeconds < 0)
        {
            return 0;
        }

        return (int)ttl.TotalSeconds;
    }
}
