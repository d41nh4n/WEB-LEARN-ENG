namespace Allen.Domain;

public class MediaModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string MediaType { get; set; } = null!;
    public string SourceUrl { get; set; } = null!;
    public QueryResult<TranscriptModel> Transcripts { get; set; } = new QueryResult<TranscriptModel>();
}
