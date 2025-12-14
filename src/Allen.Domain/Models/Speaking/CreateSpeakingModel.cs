namespace Allen.Domain;

public class CreateSpeakingModel
{
    [JsonIgnore]
    public Guid LearningUnitId { get; set; }
    [JsonIgnore]
    public Guid MediaId { get; set; }
    public CreateLearningUnitForSpeakingModel LearningUnit { get; set; } = null!;
    public CreateMediaModel Media { get; set; } = null!;
}
