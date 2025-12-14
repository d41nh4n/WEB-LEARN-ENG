namespace Allen.Domain;
public class FlashCardCreateRequestModel
{
    public List<FlashCardContentsModel> FrontContents { get; set; } = new();
    public List<FlashCardContentsModel> BackContents { get; set; } = new();
    public string? Hint { get; set; }
    public string? PersonalNotes { get; set; }
}