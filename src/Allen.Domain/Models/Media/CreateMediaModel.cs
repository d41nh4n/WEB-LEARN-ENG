namespace Allen.Domain;

public class CreateMediaModel
{
    public string? MediaType { get; set; }
    public string? Title { get; set; }
    public string SourceUrl { get; set; } = null!;
    public IEnumerable<CreateTranscriptModel> Transcripts { get; set; } = [];
}
