namespace Allen.Domain;

public class UpdateLearningUnitForSpeakingModel
{
    public Guid Id { get; set; }
    public Guid? CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Level { get; set; } = null!;
    public string? SkillType { get; set; }
}
