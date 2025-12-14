namespace Allen.Domain;

public class FlashCardsToDeckRequestModel
{
    public Guid DeckSourceId { get; set; }

    public List<Guid> FlashCardIds { get; set; } = [];
}
