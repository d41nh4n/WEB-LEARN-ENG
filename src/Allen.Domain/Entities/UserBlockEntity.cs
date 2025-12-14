namespace Allen.Domain;

[Table("UserBlocks")]
public class UserBlockEntity : EntityBase<Guid>
{
	public Guid BlockedUserId { get; set; }
	public UserEntity BlockedUser { get; set; } = null!;
	public Guid BlockedByUserId { get; set; }
	public UserEntity BlockedByUser { get; set; } = null!; 
}
