namespace Allen.Domain;

public class CreateOrUpdateQuestionModel
{
    public string? ModuleType { get; set; }
    public Guid ModuleItemId { get; set; }

    public string? QuestionType { get; set; }
    public string? Prompt { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public IFormFile? File { get; set; }
}