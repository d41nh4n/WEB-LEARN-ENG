namespace Allen.Domain;

[Table("UserAnswers")]
// For reading and listening tests
public class UserAnswerEntity : EntityBase<Guid>
{
	public Guid AttemptId { get; set; }
	public UserTestAttemptEntity Attempt { get; set; } = null!;

	public Guid QuestionId { get; set; }
	public QuestionEntity Question { get; set; } = null!;

	public Guid? SubQuestionId { get; set; }
	public SubQuestionEntity? SubQuestion { get; set; }

	public string? UserInput { get; set; }
	public bool? IsCorrect { get; set; }
}
