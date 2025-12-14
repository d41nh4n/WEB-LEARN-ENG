namespace Allen.Domain;

public class UpdateReadingParagraphModel
{
    public Guid? Id { get; set; }
    public int Order { get; set; }
    public string OriginalText { get; set; } = null!;
    public string Transcript { get; set; } = null!;
}
