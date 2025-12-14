namespace Allen.Domain;

public class CreateOrUpdateLearningUnitModel
{
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Level { get; set; } = null!;
    public string SkillType { get; set; } = null!;
	public string LearningUnitType { get; set; } = null!;
}
