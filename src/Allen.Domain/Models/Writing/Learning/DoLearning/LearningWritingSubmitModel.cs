namespace Allen.Domain;

public class LearningWritingSubmitModel
{
    public Guid UserId { get; set; }
    public Guid WritingId { get; set; }
    public string Content { get; set; } = null!;
    public int SentenceIndex { get; set; }
    public WritingModeType Mode { get; set; }
}