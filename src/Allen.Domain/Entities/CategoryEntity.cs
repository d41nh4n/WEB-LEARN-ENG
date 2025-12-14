namespace Allen.Domain;

[Table("Categories")]
public class CategoryEntity : EntityBase<Guid>
{
	public string Name { get; set; } = null!;
	public SkillType SkillType { get; set; }
	public ICollection<LearningUnitEntity> LearningUnits { get; set; } = [];
}