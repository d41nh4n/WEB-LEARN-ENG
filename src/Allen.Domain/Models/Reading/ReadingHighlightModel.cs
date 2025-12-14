namespace Allen.Domain;

public class ReadingHighlightModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public string HighlightColor { get; set; } = null!;
    public string? Note { get; set; }
}