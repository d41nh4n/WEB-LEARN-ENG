namespace Allen.Domain;

public class UpdateDeckModel
{
    public string? DeckName { get; set; }
    public string? Description { get; set; }
    public int? NumberFlashCardsPerSession { get; set; }
    public bool? IsPublic { get; set; }
    public bool? NormalModeEnabled { get; set; }
    public string? Level { get; set; }
}
