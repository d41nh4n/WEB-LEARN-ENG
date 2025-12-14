namespace Allen.Domain;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserAvatar { get; set; }
    public string? UserName { get; set; }
    public string Content { get; set; } = null!;
    public List<string>? Medias { get; set; } = new();
    public string? Privacy { get; set; }
    public int TotalReaction { get; set; }
    public int TotalComment { get; set; }
    public string? CreatedAt { get; set; }
}
