namespace Allen.Domain;

public class SpeakingModel
{
    public Guid Id { get; set; }
    public Guid LearningUnitId { get; set; }
    public MediaModel? Media { get; set; }
}
