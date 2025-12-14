namespace Allen.Domain;
public class CreateDeckModel
{
    public string? DeckName { get; set; }
    public string? Description { get; set; }
    public int? NumberFlashCardsPerSession { get; set; }
    public bool? NormalModeEnabled { get; set; }
    public bool IsPublic { get; set; } = false;
    public string? Level { get; set; }
}