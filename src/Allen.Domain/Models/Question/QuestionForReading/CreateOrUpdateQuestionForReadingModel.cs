namespace Allen.Domain;

public class CreateOrUpdateQuestionForReadingModel
{
    public Guid ModuleItemId { get; set; }
    public string? QuestionType { get; set; }
    public string? Label { get; set; }
    public string Prompt { get; set; } = null!;
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
	public int? StartTextIndex { get; set; }
	public int? EndTextIndex { get; set; }
	public string? Explanation { get; set; }

	public List<CreateSubQuestionForListeningIeltsModel>? SubQuestions { get; set; }
}
