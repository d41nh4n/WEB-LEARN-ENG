using Microsoft.AspNetCore.SignalR;

namespace Allen.Application;

[RegisterService(typeof(INotificationService))]
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(INotificationRepository repository, IUnitOfWork unitOfWork, IHubContext<NotificationHub> hub)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _hub = hub;
    }

    public async Task NotifyAsync(NotificationModel model)
    {
        if (model.UserId == model.ReceiverId)
            return;

        var entity = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            UserId = model.ReceiverId,
            EventType = model.EventType,
            Title = model.Title,
            Message = model.Message,
            ObjectId = model.ObjectId,
            ObjectType = model.ObjectType,
            PayloadJson = model.Payload != null
                ? JsonSerializer.Serialize(model.Payload)
                : null
        };

        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        await _hub.Clients.Group(model.ReceiverId.ToString()).SendAsync("ReceiveNotification",
            new { entity.Id, entity.Title, entity.Message, entity.EventType, entity.ObjectId, entity.ObjectType, entity.CreatedAt });
    }

    public async Task<QueryResult<NotificationEntity>> GetUserNotificationsAsync(Guid userId, QueryInfo queryInfo, NotificationQuery query)
    {
        return await _repository.GetUserNotificationsAsync(userId, queryInfo, query);
    }
       
    public async Task<OperationResult> MarkAsReadAsync(Guid notificationId)
    {
        var entity = await _repository.GetByIdAsync(notificationId);
        if (entity == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(NotificationEntity), notificationId));

        entity.IsRead = true;
        _repository.UpdateAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(NotificationEntity), notificationId));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(NotificationEntity), notificationId));
    }

    public async Task<OperationResult> MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _repository.GetUserNotificationsAsync(userId, new QueryInfo { NeedTotalCount = false }, new NotificationQuery { IsRead = false });
        foreach (var n in notifications.Data)
        {
            n.IsRead = true;
            _repository.UpdateAsync(n);
        }

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(NotificationEntity), userId));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(NotificationEntity), userId));
    }

    public async Task<OperationResult> DeleteAsync(Guid notificationId)
    {
        await _repository.DeleteByIdAsync(notificationId);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(NotificationEntity), notificationId));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(NotificationEntity), notificationId));
    }
}