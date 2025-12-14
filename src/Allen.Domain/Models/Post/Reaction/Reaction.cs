namespace Allen.Domain;

public class Reaction
{
    public Guid Id { get; set; }
    public Guid ObjectId { get; set; }
    public string? ObjectType { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserPicture { get; set; }
    public string? ReactionType { get; set; }
}
