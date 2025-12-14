namespace Allen.Domain;

public class UpdateIeltsWritingModel
{
    public UpdateLearningUnitForWritingModel LearningUnit { get; set; } = null!;
    public string? TaskType { get; set; }
    public IFormFile? SourceUrl { get; set; }
    public string? ContentEN { get; set; }
    public string? Hint { get; set; }
}
