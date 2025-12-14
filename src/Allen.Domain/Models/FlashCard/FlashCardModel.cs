namespace Allen.Domain;
public class FlashCardModel
{
    public Guid Id { get; set; }
    public Guid DeckId { get; set; }

    public List<FlashCardContentsModel> Front { get; set; } = new();
    public List<FlashCardContentsModel> Back { get; set; } = new();

    public string? Hint { get; set; }
    public string? PersonalNotes { get; set; }
    public bool IsSuspended { get; set; }
    public DateTime? LastClonedDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }

    // FSRS fields
    public FlashCardStateModel? CardState { get; set; }
}