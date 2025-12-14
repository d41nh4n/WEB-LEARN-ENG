namespace Allen.Domain;

public class ReactionSummaryModel
{
    public string ReactionType { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<ReactionUserModel> Users { get; set; } = new();
}