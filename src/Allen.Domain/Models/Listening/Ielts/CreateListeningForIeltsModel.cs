namespace Allen.Domain;

public class CreateListeningForIeltsModel
{
	public Guid LearningUnitId { get; set; }
	public CreateMediaWithoutTranscriptModel Media { get; set; } = default!;
	public int? SectionIndex { get; set; }
	public int? EstimatedReadingTime { get; set; }
}
