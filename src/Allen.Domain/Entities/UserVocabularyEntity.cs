namespace Allen.Domain;

[Table("UserVocabularies")]
public class UserVocabularyEntity : EntityBase<Guid>
{
	public Guid UserId { get; set; }
	public UserEntity User { get; set; } = null!;
	public Guid VocabularyId { get; set; }
	public VocabularyEntity Vocabulary { get; set; } = null!;
}
