namespace Allen.Application;

public interface ICommentsService
{
    Task<QueryResult<Comment>> GetRootCommentsAsync(Guid postId, QueryInfo queryInfo);
    Task<QueryResult<Comment>> GetRepliesAsync(Guid parentId, QueryInfo queryInfo);
    Task<Comment> GetCommentByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateCommentModel model);
    Task<OperationResult> UpdateAsync(Guid id, UpdateCommentModel model);
    Task<OperationResult> DeleteAsync(Guid id, Guid userId);
}
