namespace Allen.Domain;

public class HistoryModel
{
	public Guid Id { get; set; }
	public Guid LearningUnitId { get; set; }
	public string Title { get; set; } = string.Empty;
	public DateTime SubmitAt { get; set; }
	public double TimeToDo { get; set; }
	public int CorrectAnswers { get; set; }
	public int UnCorrectAnswers { get; set; }
	public int TotalQuestions { get; set; }
	public double PercentageCorrect { get; set; }
	public string Type { get; set; } = string.Empty;
	public double Score { get; set; }
}
