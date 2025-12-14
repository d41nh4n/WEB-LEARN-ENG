namespace Allen.Domain;

public class UpdateMediaWithoutTranscriptModel
{
    public Guid Id { get; set; }
    public string? MediaType { get; set; }
    public string SourceUrl { get; set; } = null!;
}
