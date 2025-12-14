namespace Allen.Domain;

public class GetQuestionForListeningIeltsModel
{
    public Guid Id { get; set; }
    public Guid ModuleItemId { get; set; }
    public string? QuestionType { get; set; }
    public string Prompt { get; set; } = null!;
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public DateTime? CreateAt { get; set; }
    public List<GetSubQuestionForListeningIeltsModel>? SubQuestions { get; set; }
}
