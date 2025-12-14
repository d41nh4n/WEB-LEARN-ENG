namespace Allen.Domain;

public class UpdateQuestionModel
{
    public Guid Id { get; set; }
    public string? QuestionType { get; set; }
    public string? Prompt { get; set; }
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public List<UpdateSubQuestionModel> SubQuestions { get; set; } = new List<UpdateSubQuestionModel>();
}
