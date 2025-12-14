namespace Allen.Domain;

public class NotificationModel
{
    public Guid ReceiverId { get; set; }      // người nhận thông báo
    public Guid  UserId { get; set; }     // người thực hiện hành động
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public Guid? ObjectId { get; set; }
    public ObjectType? ObjectType { get; set; }
    public object? Payload { get; set; }
}