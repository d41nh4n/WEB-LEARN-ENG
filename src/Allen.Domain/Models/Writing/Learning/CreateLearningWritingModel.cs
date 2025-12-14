namespace Allen.Domain;

public class CreateLearningWritingModel
{
    [JsonIgnore]
    public Guid LearningUnitId { get; set; }
    public CreateLearningUnitForWritingModel LearningUnit { get; set; } = null!;
    public string ContentVN { get; set; } = null!;
    public string ContentEN { get; set; } = null!;
}