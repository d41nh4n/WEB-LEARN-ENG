namespace Allen.Domain;

public class CreateIeltsReadingPassageModel
{
    public Guid LearningUnitId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? EstimatedReadingTime { get; set; }
    public int? ReadingPassageNumber { get; set; }
}
