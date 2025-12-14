namespace Allen.Domain;

public class ReadingPassageForIeltsModel
{
    public Guid Id { get; set; }
	public Guid LearningUnitId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? EstimatedReadingTime { get; set; }
    public int? ReadingPassageNumber { get; set; }
    public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
}
