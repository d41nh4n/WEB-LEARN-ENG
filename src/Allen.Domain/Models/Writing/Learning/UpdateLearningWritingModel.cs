namespace Allen.Domain;

public class UpdateLearningWritingModel
{
    public UpdateLearningUnitForWritingModel LearningUnit { get; set; } = null!;
    public string ContentVN { get; set; } = null!;
    public string ContentEN { get; set; } = null!;
}