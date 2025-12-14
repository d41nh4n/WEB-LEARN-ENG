namespace Allen.Domain;

public class CreateSubQuestionModel
{
	[JsonIgnore]
    public Guid QuestionId { get; set; }
    public string Label { get; set; } = null!;
	public string? Prompt { get; set; }
	public List<string>? Options { get; set; }
	public string? CorrectAnswer { get; set; }
}
