namespace Allen.Domain;

[Table("Reactions")]
public class ReactionEntity : EntityBase<Guid>
{
	public Guid ObjectId { get; set; }
	public ObjectType ObjectType { get; set; }
	public Guid UserId { get; set; }
	public UserEntity User { get; set; } = null!;
	public ReactionType ReactionType { get; set; }
}
