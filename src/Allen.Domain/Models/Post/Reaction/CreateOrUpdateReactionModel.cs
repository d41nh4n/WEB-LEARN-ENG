namespace Allen.Domain;

public class CreateOrUpdateReactionModel
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Guid ObjectId { get; set; }
    public string? ReactionType { get; set; }
}