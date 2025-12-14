namespace Allen.Domain;

public class SpeakingEntity : EntityBase<Guid>
{
    public Guid LearningUnitId { get; set; }
    public LearningUnitEntity? LearningUnit { get; set; }
    public Guid? MediaId { get; set; }
    public MediaEntity? Media { get; set; }
	public int? SectionIndex { get; set; }
}
