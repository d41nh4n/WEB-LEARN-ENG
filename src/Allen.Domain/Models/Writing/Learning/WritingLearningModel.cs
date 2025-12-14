namespace Allen.Domain;

public class WritingLearningModel
{
    public Guid Id { get; set; }
    public Guid LearningUnitId { get; set; }
    public string ContentVN { get; set; } = null!;
    public string ContentEN { get; set; } = null!;

    public CreateLearningUnitForWritingModel? LearningUnit { get; set; }
}
