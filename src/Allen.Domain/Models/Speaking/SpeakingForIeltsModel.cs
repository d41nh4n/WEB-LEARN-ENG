namespace Allen.Domain;

public class SpeakingForIeltsModel
{
	public Guid Id { get; set; }
	public Guid LearningUnitId { get; set; }
	public int? SectionIndex { get; set; }
	public QuestionForSpeakingModel? Question { get; set; }
}
