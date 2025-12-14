namespace Allen.Domain;

[Table("Follows")]
public class FollowEntity : EntityBase<Guid>
{
	public Guid FollowerId { get; set; }
	public UserEntity Follower { get; set; } = null!;

	public Guid FollowingId { get; set; }
	public UserEntity Following { get; set; } = null!;
}
