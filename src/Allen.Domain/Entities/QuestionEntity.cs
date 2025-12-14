namespace Allen.Domain;

[Table("Questions")]
public class QuestionEntity : EntityBase<Guid>
{
	public LearningModuleType ModuleType { get; set; }
	public Guid ModuleItemId { get; set; } 
    public QuestionType QuestionType { get; set; }
	public string? Prompt { get; set; }
	public string? Options { get; set; }
	public string? CorrectAnswer { get; set; }
	public string? ContentUrl { get; set; }
    public string? TableMetadata { get; set; } 
	public string? Label { get; set; }
	public int? StartTextIndex { get; set; }
	public int? EndTextIndex { get; set; }

	public int? StartTranscriptIndex { get; set; }
	public int? EndTranscriptIndex { get; set; }
	public string? Explanation { get; set; }
	public ICollection<SubQuestionEntity>? SubQuestions { get; set; }
	public ICollection<UserAnswerEntity> UserAnswers { get; set; } = [];
}
