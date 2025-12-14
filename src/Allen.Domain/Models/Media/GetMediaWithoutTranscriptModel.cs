namespace Allen.Domain;

public class GetMediaWithoutTranscriptModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string MediaType { get; set; } = null!;
    public string SourceUrl { get; set; } = null!;
}
