namespace Allen.Domain;

public class QuestionModel
{
    public Guid Id { get; set; }
    public string QuestionType { get; set; } = null!;
    public string? ContentUrl { get; set; }
    public Guid ModuleItemId { get; set; }
    public string? Label { get; set; }
    public string? Prompt { get; set; }
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public Table? TableMetadata { get; set; }
    public List<SubQuestionModel> SubQuestions { get; set; } = [];
}
