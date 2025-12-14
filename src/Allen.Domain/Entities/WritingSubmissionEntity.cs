namespace Allen.Domain;

[Table("WritingSubmissions")]
public class WritingSubmissionEntity : EntityBase<Guid>
{
	public Guid UserId { get; set; }
	public UserEntity User { get; set; } = null!;

	public Guid AttemptId { get; set; }
	public UserTestAttemptEntity Attempt { get; set; } = null!;

	public Guid WritingId { get; set; }
	public WritingEntity Writing { get; set; } = null!;
	public string Content { get; set; } = null!;
	public int TaskResponse { get; set; }
	public int CoherenceCohesion { get; set; }
	public int LexicalResource { get; set; }
	public int GrammaticalAccuracy { get; set; }
}