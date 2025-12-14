using Allen.Common.Settings.Enum;
namespace Allen.Domain;

public class DeckModel
{
    public Guid Id { get; set; }
    public string? DeckName { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public int TotalUsersUsing { get; set; } = 0;
    public bool IsClone { get; set; } = false;
    public string? Level { get; set; }
    public int TotalFlashCard { get; set; } = 0;
}
