namespace Allen.Domain;
public class SubmitIeltsModel
{
    public Guid LearningUnitId { get; set; }
    [JsonIgnore]
    public Guid UserId { get; set; }
    public double TimeSpent { get; set; }
	public List<AnswerRequest> Answers { get; set; } = new();
}
public class AnswerRequest
{
    public Guid QuestionId { get; set; }
    public Guid? SubQuestionId { get; set; }
    public string? Answer { get; set; }
}

public class SubmissionResult
{
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public double Score { get; set; }
    public double Band { get; set; }
    public List<AnswerResult> Details { get; set; } = new();
}

public class AnswerResult
{
    public Guid QuestionId { get; set; }
    public Guid? SubQuestionId { get; set; }
    public string? Label { get; set; }
    public string? UserAnswer { get; set; }
    public string? CorrectAnswer { get; set; }
    public bool IsCorrect { get; set; }
	public int? StartTextIndex { get; set; }
	public int? EndTextIndex { get; set; }

	public int? StartTranscriptIndex { get; set; }
	public int? EndTranscriptIndex { get; set; }
	public string? Explanation { get; set; }
}