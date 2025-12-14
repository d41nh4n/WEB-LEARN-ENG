namespace Allen.Domain;

[Table("Listenings")]
public class ListeningEntity : EntityBase<Guid>
{
    public Guid LearningUnitId { get; set; }
    public LearningUnitEntity? LearningUnit { get; set; }
    public Guid MediaId { get; set; }
    public MediaEntity? Media { get; set; }
    public int? SectionIndex { get; set; }
    public int? EstimatedReadingTime { get; set; } // minutes
    [NotMapped]
    public ICollection<QuestionEntity>? Questions { get; set; }
}