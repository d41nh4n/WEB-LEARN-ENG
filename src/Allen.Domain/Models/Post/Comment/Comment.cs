namespace Allen.Domain;

public class Comment
{
    public Guid Id { get; set; }
    public Guid ObjectId { get; set; }
    public Guid UserId { get; set; }
    public string? UserAvatar { get; set; }
    public string? UserName { get; set; }
    public string Content { get; set; } = string.Empty;

    public Guid CommentParentId { get; set; }
    public Guid? ParentId { get; set; }
    public string? ReplyToUserName { get; set; }
    public int ReplyCount { get; set; }
    public int TotalReaction { get; set; } = 0;
    public DateTime? CreatedAt { get; set; }
}
