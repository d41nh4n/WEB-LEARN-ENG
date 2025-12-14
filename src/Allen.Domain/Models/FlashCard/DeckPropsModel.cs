namespace Allen.Domain;

public class DeckPropsModel
{
    public int NumberFlashCardsPerSession { get; set; } = 20;
    public bool NormalModeEnabled { get; set; } = true; //option to enable/disable normal mode (normal: full card, un-normal: SRS)
    public bool IsPublic { get; set; } = false;
    public double DesiredRetention { get; set; }
}
