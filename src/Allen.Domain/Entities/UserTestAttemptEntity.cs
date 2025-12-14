namespace Allen.Domain;

[Table("UserTestAttempts")]
public class UserTestAttemptEntity : EntityBase<Guid>
{
	public Guid UserId { get; set; }
	public UserEntity User { get; set; } = null!;
	public Guid LearningUnitId { get; set; }
	public decimal OverallBand { get; set; }
    public int TotalCorrect { get; set; }
	public int TotalQuestions { get; set; }
	public double TimeSpent { get; set; }
	public ICollection<UserAnswerEntity> UserAnswers { get; set; } = [];
	public ICollection<WritingSubmissionEntity> WritingSubmissions { get; set; } = [];
}
