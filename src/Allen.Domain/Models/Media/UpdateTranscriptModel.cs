namespace Allen.Domain;

public class UpdateTranscriptModel
{
    public Guid Id { get; set; }
    public Guid MediaId { get; set; }
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public string ContentEN { get; set; } = null!;
    public string ContentVN { get; set; } = null!;
    public string? IPA { get; set; }
    public int OrderIndex { get; set; }
}
