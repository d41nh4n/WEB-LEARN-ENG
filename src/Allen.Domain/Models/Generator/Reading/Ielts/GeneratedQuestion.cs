namespace Allen.Domain;

public class GeneratedQuestion
{
	public Guid QuestionId { get; set; }
	public QuestionType Type { get; set; }
	public string? Question { get; set; }
	public List<string>? Options { get; set; } // For MultipleChoice, Matching
	public string? CorrectAnswer { get; set; }
	public int StartIndex { get; set; } // Vị trí bắt đầu evidence trong passage
	public int EndIndex { get; set; } // Vị trí kết thúc evidence
	public string? Explanation { get; set; }
}
