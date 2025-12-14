namespace Allen.Domain;

public class CreateOrUpdateQuestionForListeningModel
{
    public Guid ModuleItemId { get; set; }
    public string? QuestionType { get; set; }
    public string? Label { get; set; }
    public string Prompt { get; set; } = null!;
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public Table? TableMetadata { get; set; }
    public string? ContentUrl { get; set; }
    public List<CreateSubQuestionForListeningIeltsModel>? SubQuestions { get; set; }
}
