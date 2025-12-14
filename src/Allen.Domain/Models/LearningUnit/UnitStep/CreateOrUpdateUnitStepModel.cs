namespace Allen.Domain;

public class CreateOrUpdateUnitStepModel
{
    public Guid LearningUnitId { get; set; }
    [JsonIgnore]
    public int StepIndex { get; set; }
    public string? Title { get; set; }
    public string? ContentJson { get; set; }
}
