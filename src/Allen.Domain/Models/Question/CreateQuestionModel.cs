namespace Allen.Domain;

public class CreateQuestionModel
{
	[JsonIgnore]
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid ModuleItemId { get; set; }
	[JsonIgnore]
    public LearningModuleType ModuleType { get; set; }
    public string? QuestionType { get; set; }
	public string Prompt { get; set; } = null!;
	public List<string>? Options { get; set; }
	public string? CorrectAnswer { get; set; }

	// Cho dạng gap-fill hoặc matching
	public List<CreateSubQuestionModel>? SubQuestions { get; set; }
}
