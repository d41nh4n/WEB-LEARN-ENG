namespace Allen.Application;

public interface IPostsService
{
    Task<QueryResult<Post>> GetPostsWithPagingAsync(GetPostQuery postQuery, QueryInfo queryInfo);
    Task<QueryResult<Post>> GetMyPostsAsync(GetPostQuery postQuery, QueryInfo queryInfo);
    Task<Post> GetByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreatePostModel model);
    Task<OperationResult> UpdateAsync(Guid id, UpdatePostModel model);
    Task<OperationResult> DeleteAsync(Guid id, Guid userId);
}

