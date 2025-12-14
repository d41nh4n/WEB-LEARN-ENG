namespace Allen.Infrastructure;

public interface ICommentsRepository : IRepositoryBase<CommentEntity>
{
    Task<CommentEntity> GetCommentByIdAsync(Guid id);
    Task<QueryResult<Comment>> GetRootCommentsAsync(Guid postId, QueryInfo queryInfo);
    Task<QueryResult<Comment>> GetRepliesAsync(Guid rootCommentId, QueryInfo queryInfo);

    Task<List<CommentEntity>> GetCommentsByParentIdAsync(Guid parentId);
    Task ReassignRepliesParentAsync(CommentEntity comment, CommentEntity newParent);
}
