namespace Allen.API.Controllers;

[Route("comments")]
[Authorize]
public class CommentsController(ICommentsService _service) : BaseApiController
{
    [HttpGet("root/{objectId}")]
    public async Task<QueryResult<Comment>> GetRootComments(Guid objectId, [FromQuery] QueryInfo queryInfo)
        => await _service.GetRootCommentsAsync(objectId, queryInfo);

    [HttpGet("replies/{parentId}")]
    public async Task<QueryResult<Comment>> GetReplies(Guid parentId, [FromQuery] QueryInfo queryInfo)
        => await _service.GetRepliesAsync(parentId, queryInfo);

    [HttpGet("{id}")]
    public async Task<Comment?> GetById(Guid id)
    => await _service.GetCommentByIdAsync(id);


    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateCommentModel model)
    {
        model.UserId = HttpContext.GetCurrentUserId();
        model.UserName = HttpContext.GetCurrentUserName();
        return await _service.CreateAsync(model);
    }

    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update(Guid id, [FromBody] UpdateCommentModel model)
    {
        model.UserId = HttpContext.GetCurrentUserId();
        return await _service.UpdateAsync(id, model);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete(Guid id)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _service.DeleteAsync(id, userId);
    }
}
