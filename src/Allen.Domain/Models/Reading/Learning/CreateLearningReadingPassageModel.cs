namespace Allen.Domain;

public class CreateLearningReadingPassageModel
{
    [JsonIgnore]
	public Guid LearningUnitId { get; set; }
    public required CreateLearningUnitForReadingModel LearningUnit { get; set; } = null!;
    // Danh sách đoạn + transcript
    public List<CreateReadingParagraphModel> Paragraphs { get; set; } = new();
}
