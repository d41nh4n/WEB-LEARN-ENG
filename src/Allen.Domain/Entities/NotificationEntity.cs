namespace Allen.Domain;

[Table("Notifications")]
public class NotificationEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    public Guid? ObjectId { get; set; }
    public ObjectType? ObjectType { get; set; }

    public string EventType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;

    public string? PayloadJson { get; set; }
    public bool IsRead { get; set; } = false;
}