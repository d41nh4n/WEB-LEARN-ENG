namespace Allen.Domain;

public class FlashCardUpdateRequestModel
{
    public List<FlashCardContentsModel>? FrontContents { get; set; }
    public List<FlashCardContentsModel>? BackContents { get; set; }
    public string? Hint { get; set; }
    public string? PersonalNotes { get; set; }
    public bool? IsSuspended { get; set; }
}
