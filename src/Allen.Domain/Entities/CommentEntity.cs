namespace Allen.Domain;

[Table("Comments")]
public class CommentEntity : EntityBase<Guid>
{
	public Guid ObjectId { get; set; }
	public ObjectType ObjectType { get; set; }

	public Guid UserId { get; set; }
	public UserEntity? User { get; set; }

	public Guid CommentParentId { get; set; }
	public CommentEntity? Parent { get; set; }            // <-- parent comment

	[Required]
	[MaxLength(AppConstants.MaxLengthTopic)]
	public string Content { get; set; } = string.Empty;

	public ICollection<CommentEntity> Children { get; set; } = [];
}
