namespace Allen.Domain;

public class CreateLearningUnitForSpeakingModel
{
    public Guid? CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Level { get; set; } = null!;
    public string? SkillType { get; set; }
}
