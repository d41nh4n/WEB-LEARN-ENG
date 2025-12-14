namespace Allen.Domain;

public class WritingIeltsModel
{
    public Guid Id { get; set; }
    public Guid LearningUnitId { get; set; }

    public string? TaskType { get; set; }
    public string? SourceUrl { get; set; }
    public string? ContentEN { get; set; }
    public string? Hint { get; set; }

    public CreateLearningUnitForWritingModel? LearningUnit { get; set; }
}
