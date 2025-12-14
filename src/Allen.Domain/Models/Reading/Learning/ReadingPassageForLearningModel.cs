namespace Allen.Domain;

public class ReadingPassageForLearningModel
{
    public Guid Id { get; set; }
    public Guid LearningUnitId { get; set; }
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Level { get; set; } = null!;
    public List<ReadingParagraphModel> Paragraphs { get; set; } = [];
}
