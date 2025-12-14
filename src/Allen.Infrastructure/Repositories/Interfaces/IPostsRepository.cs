namespace Allen.Infrastructure;

public interface IPostsRepository : IRepositoryBase<PostEntity>
{
    Task<QueryResult<Post>> GetPostsWithPagingAsync(GetPostQuery postQuery, QueryInfo queryInfo);
    Task<QueryResult<Post>> GetMyPostsAsync(GetPostQuery query, QueryInfo queryInfo);
    Task<Post?> GetPostByIdAsync(Guid id);
}