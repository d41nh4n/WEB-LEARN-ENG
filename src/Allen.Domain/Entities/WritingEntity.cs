namespace Allen.Domain;

[Table("Writings")]
public class WritingEntity : EntityBase<Guid>
{
    public Guid LearningUnitId { get; set; }
    public LearningUnitEntity LearningUnit { get; set; } = null!;

    public WritingTaskType? TaskType { get; set; }
    public string? ContentVN { get; set; }
    public string? ContentEN { get; set; }
    public string? SourceUrl { get; set; }
    public string? Hint { get; set; }
}