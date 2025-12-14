namespace Allen.Domain;

public class GetCommentQuery
{
    public Guid ObjectId { get; set; }
    public Guid? RootCommentId { get; set; }
}
