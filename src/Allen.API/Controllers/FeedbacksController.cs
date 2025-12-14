namespace Allen.API.Controllers;

[Route("feedbacks")]
[Authorize]
public class FeedbacksController(IFeedbacksService _service) : BaseApiController
{
	[HttpGet]
	[ValidateModel]
	public async Task<QueryResult<FeedbackModel>> GetFeedbacks([FromQuery] FeedbackQuery query, [FromQuery] QueryInfo queryInfo)
	{
		return await _service.GetFeedbacksAsync(query, queryInfo);
	}
	[HttpGet("me")]
	[ValidateModel]
	public async Task<QueryResult<FeedbackModel>> GetFeedbacksOfUser([FromQuery] FeedbackQuery query, [FromQuery] QueryInfo queryInfo)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _service.GetFeedbacksOfUserAsync(userId, query, queryInfo);
	}

	[HttpGet("{id}")]
	public async Task<FeedbackModel> GetById([FromRoute] Guid id)
	{
		return await _service.GetByIdAsync(id);
	}
	//[HttpGet("{id}/me")]
	//public async Task<FeedbackModel> GetByIdOfUser([FromRoute] Guid id)
	//{
	//	var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
	//	return await _service.GetByIdOfUserAsync(userId, id);
	//}
	[HttpPost]
	[ValidateModel]
	public async Task<OperationResult> CreateFeedback([FromForm] CreateOrUpdateFeedbackModel model)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _service.CreateAsync(userId,model);
	}
	[HttpPatch("{id}")]
	[ValidateModel]
	public async Task<OperationResult> UpdateFeedback([FromRoute] Guid id, [FromForm] CreateOrUpdateFeedbackModel model)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _service.UpdateAsync(id, userId ,model);
	}
	[HttpDelete("{id}")]
	public async Task<OperationResult> DeleteFeedback([FromRoute] Guid id)
	{
		return await _service.DeleteAsync(id);
	}
}
