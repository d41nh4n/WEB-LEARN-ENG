namespace Allen.Domain;

public class LearningWritingResponseModel
{
    public bool IsCorrect { get; set; }
    public decimal CurrentScore { get; set; }
    public GeminiFeedback Feedback { get; set; } = new();
}