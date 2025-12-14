namespace Allen.Domain;

public class PronunciationModel
{
    public IFormFile AudioFile { get; set; } = null!;
    public string ReferenceText { get; set; } = string.Empty;
    public string? Accent { get; set; } = "US";
}
