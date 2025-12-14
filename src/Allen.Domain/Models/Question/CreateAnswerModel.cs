namespace Allen.Domain;

public class CreateAnswerModel
{
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public string? UserInput { get; set; }
    public bool? IsCorrect { get; set; }
    public decimal? Score { get; set; }
    public string? AiErrorFeedback { get; set; }
}
