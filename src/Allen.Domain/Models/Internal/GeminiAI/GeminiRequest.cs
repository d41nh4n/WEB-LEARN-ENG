namespace Allen.Domain;

public class GeminiRequest
{
    public string? Prompt { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}