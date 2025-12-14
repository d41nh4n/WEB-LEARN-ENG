namespace Allen.Domain;

public class FlashCardStudyModel
{
    public Guid Id { get; set; }
    public List<FlashCardContentsModel> Front { get; set; } = new();
    public List<FlashCardContentsModel> Back { get; set; } = new();

    public string? Hint { get; set; }
    public string? PersonalNotes { get; set; }
}
