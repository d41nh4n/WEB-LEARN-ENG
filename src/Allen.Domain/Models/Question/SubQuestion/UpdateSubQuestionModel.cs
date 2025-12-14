namespace Allen.Domain;

public class UpdateSubQuestionModel
{
    public Guid Id { get; set; }
    public string Label { get; set; } = null!;
    public string? Prompt { get; set; }
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
}
