namespace Allen.Domain;

public class UpdateCommentModel
{
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
