namespace Allen.Domain;

[Table("ReadingPassages")]
public class ReadingPassageEntity : EntityBase<Guid>
{
	public Guid LearningUnitId { get; set; }
	public LearningUnitEntity LearningUnit { get; set; } = null!;
	public string? Content { get; set; }
    public string? Title { get; set; }
    public int? EstimatedReadingTime { get; set; }
    public int? ReadingPassageNumber { get; set; }
	public ICollection<ReadingParagraphEntity> Paragraphs { get; set; } = [];
	//public ICollection<ReadingHighlightEntity> ReadingHighlights { get; set; } = [];

    [NotMapped]
    public ICollection<QuestionEntity> Questions { get; set; } = [];
}
