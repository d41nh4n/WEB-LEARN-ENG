namespace Allen.Domain;

[Table("SubQuestions")]
public class SubQuestionEntity : EntityBase<Guid>
{
	public Guid QuestionId { get; set; }
	public QuestionEntity? Question { get; set; }

	// Ví dụ: "Gap 1", "Heading Match A"
	public string? Label { get; set; }

	public string? Prompt { get; set; }  // Nếu có
	public string? CorrectAnswer { get; set; }
	public string? Options { get; set; } // Nếu matching hoặc MCQ
	public int? StartTextIndex { get; set; }
	public int? EndTextIndex { get; set; }
	public int? StartTranscriptIndex { get; set; }
	public int? EndTranscriptIndex { get; set; }
	public string? Explanation { get; set; }
}
