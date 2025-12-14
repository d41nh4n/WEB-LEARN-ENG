namespace Allen.Domain;

public class LearningUnitQuery
{
    public Guid? CategoryId { get; set; }
    public string? SkillType { get; set; }
    public string? LevelType { get; set; }
    public string? LearningUnitType { get; set; }
    public string? LearningUnitStatusType { get; set; }
    public string? TaskType { get; set; }
}
