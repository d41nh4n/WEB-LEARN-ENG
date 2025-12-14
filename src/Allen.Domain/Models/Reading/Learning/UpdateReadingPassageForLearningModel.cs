namespace Allen.Domain;

public class UpdateReadingPassageForLearningModel
{
    public required UpdateLearningUnitForWritingModel LearningUnit { get; set; }
    public List<UpdateReadingParagraphModel> Paragraphs { get; set; } = new();
}
