namespace Allen.Domain;

[Table("Transcripts")]
public class TranscriptEntity : EntityBase<Guid>
{
    public Guid MediaId { get; set; }
    public MediaEntity Media { get; set; } = null!;
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public string ContentEN { get; set; } = null!;
    public string ContentVN { get; set; } = null!;
    public string? IPA { get; set; }
    public int OrderIndex { get; set; }
}