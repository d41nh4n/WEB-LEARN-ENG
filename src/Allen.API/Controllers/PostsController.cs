namespace Allen.API.Controllers;

[Route("posts")]
public class PostsController(IPostsService _postsService) : BaseApiController
{
    [HttpGet("myPost")]
    public async Task<QueryResult<Post>> GetMyPosts([FromQuery] GetPostQuery postQuery, [FromQuery] QueryInfo queryInfo)
    {
        postQuery.UserId = HttpContext.GetCurrentUserId();
        return await _postsService.GetMyPostsAsync(postQuery, queryInfo);
    }

    [HttpGet("paging")]
    [ValidateModel]
    public async Task<QueryResult<Post>> GetPostsWithPaging([FromQuery] GetPostQuery postQuery, [FromQuery] QueryInfo queryInfo)
    {
        postQuery.UserId = HttpContext.GetCurrentUserId();
        return await _postsService.GetPostsWithPagingAsync(postQuery, queryInfo);
    }

    [HttpGet("{id}")]
    public async Task<Post> GetById(Guid id)
    {
        return await _postsService.GetByIdAsync(id);
    }

    [HttpPost]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> Create([FromForm] CreatePostModel model)
    {
        model.UserId = HttpContext.GetCurrentUserId();
        return await _postsService.CreateAsync(model);
    }

    [HttpPatch("{id}")]
    [ValidateModel]
    [Authorize]
    public async Task<OperationResult> Update(Guid id, [FromForm] UpdatePostModel model)
    {
        model.UserId = HttpContext.GetCurrentUserId();
        return await _postsService.UpdateAsync(id, model);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete(Guid id)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _postsService.DeleteAsync(id, userId);
    }
}