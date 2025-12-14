namespace Allen.Domain;

public class ReadingParagraphModel
{
    public Guid Id { get; set; }
    public int Order { get; set; }                
    public string OriginalText { get; set; } = null!;
    public string Transcript { get; set; } = null!;
}
