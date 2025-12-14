namespace Allen.Domain;

public class CreateCommentModel
{
    public Guid ObjectId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Guid? CommentParentId { get; set; }
    public string Content { get; set; } = string.Empty;
}