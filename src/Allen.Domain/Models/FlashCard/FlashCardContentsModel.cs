namespace Allen.Domain;
public class FlashCardContentsModel
{
    // can be [text, image, audio]
    public string? Type { get; set; }
    public string? Text { get; set; } = string.Empty;
}