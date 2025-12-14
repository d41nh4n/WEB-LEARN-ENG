namespace Allen.Domain;

[Table("UserPoints")]
public class UserPointEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public int TotalPoints { get; set; }
}