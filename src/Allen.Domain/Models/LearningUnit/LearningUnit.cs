namespace Allen.Domain;

public class LearningUnit
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Level { get; set; } = null!;
    public string? SkillType { get; set; }
    public string? Description { get; set; }
	public DateTime? CreateAt { get; set; }
    public List<UnitStep>? UnitSteps { get; set; } = [];
}
