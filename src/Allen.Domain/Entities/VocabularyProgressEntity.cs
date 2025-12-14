namespace Allen.Domain;

[Table("VocabularyProgress")]
public class VocabularyProgressEntity : EntityBase<Guid>
{
	public Guid UserId { get; set; }
	public UserEntity User { get; set; } = null!;
	public Guid VocabularyId { get; set; }
	public VocabularyEntity Vocabulary { get; set; } = null!;
	public ActivityType? ActivityType { get; set; }
	public decimal? Score { get; set; }
}
