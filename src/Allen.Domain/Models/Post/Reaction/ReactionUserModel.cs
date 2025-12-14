namespace Allen.Domain;

public class ReactionUserModel
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserPicture { get; set; }
}
