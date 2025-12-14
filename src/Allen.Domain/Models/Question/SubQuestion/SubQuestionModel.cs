namespace Allen.Domain;

public class SubQuestionModel
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Label { get; set; } = default!;
    public string? Prompt { get; set; }
    public string? CorrectAnswer { get; set; }
    public List<string>? Options { get; set; }
}
