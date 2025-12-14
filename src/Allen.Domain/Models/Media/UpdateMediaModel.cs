namespace Allen.Domain;

public class UpdateMediaModel
{
    public Guid Id { get; set; }
    public string? MediaType { get; set; }
    public string? Title { get; set; }
    public string SourceUrl { get; set; } = null!;
    public IEnumerable<UpdateTranscriptModel> Transcripts { get; set; } = [];
}
