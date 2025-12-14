namespace Allen.Domain;

public class CreateSubQuestionForListeningIeltsModel
{
    public Guid? Id { get; set; }
    [JsonIgnore]
    public Guid QuestionId { get; set; }
    public string? Label { get; set; }
    public string? Prompt { get; set; }
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
	public int? StartTextIndex { get; set; }
	public int? EndTextIndex { get; set; }
	public string? Explanation { get; set; }
}
