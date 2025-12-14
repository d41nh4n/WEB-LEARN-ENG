namespace Allen.Domain;

[Table("PasswordResetTokens")]
public class PasswordResetTokenEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    [MaxLength(256)]
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
    public bool IsUsed { get; set; } = false;
}
