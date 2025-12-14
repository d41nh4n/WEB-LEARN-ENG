namespace Allen.Domain;

[Table("PushSubscriptions")]
public class PushSubscriptionEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public string? Endpoint { get; set; }
    public string? P256DH { get; set; }
    public string? Auth { get; set; }
    public string? UserAgent { get; set; }
}