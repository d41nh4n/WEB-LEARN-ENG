namespace Allen.Domain;

[Table("Medias")]
public class MediaEntity : EntityBase<Guid>
{
    public string? Title { get; set; }
    public MediaType MediaType { get; set; }
    public string SourceUrl { get; set; } = null!;
    public ICollection<TranscriptEntity> Transcripts { get; set; } = [];
}