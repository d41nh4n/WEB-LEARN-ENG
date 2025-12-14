namespace Allen.API.Controllers;

[Route("listenings")]
[Authorize]
public class ListeningsController(
	IListeningsService _service) : BaseApiController
{
	[HttpGet("learning/{learningUnitId}/learningunit")]
	[AllowAnonymous]
	public async Task<ListeningModel> GetByLearningUnitId([FromRoute] Guid learningUnitId)
	  => await _service.GetByLearningUnitIdAsync(learningUnitId);

	[HttpPost("learning")]
	[ValidateModel]
	public async Task<OperationResult> CreateForLearning([FromBody] CreateListeningForLearningModel model)
	=> await _service.CreateLearningAsync(model);

	[HttpPatch("learning/{id}")]
	[ValidateModel]
	public async Task<OperationResult> UpdateForLearning(Guid id, [FromBody] UpdateListeningForLearningModel model)
	=> await _service.UpdateLearningAsync(id, model);

	[HttpGet("ielts/{learningUnitId}/learningunit")]
	[AllowAnonymous]
	public async Task<ListeningForIeltsModel> GetByLearningUnitIdForIelts([FromRoute] Guid learningUnitId, [FromQuery] GetByLearningUnitIdForIeltsQuery query)
	  => await _service.GetByLearningUnitIdForIeltsAsync(learningUnitId, query);

	[HttpPost("ielts")]
	[ValidateModel]
	public async Task<OperationResult> CreateListeningForIelts([FromBody] CreateListeningForIeltsModel model)
		=> await _service.CreateListeningForIeltsAsync(model);

	[HttpPost("ielts-bulk")]
	[ValidateModel]
	public async Task<OperationResult> CreateListeningsForIelts([FromBody] CreateListeningsForIeltsModel model)
		=> await _service.CreateListeningsForIeltsAsync(model);

	[HttpPost("ielts/submit")]
	[Authorize(Roles = "User")]
	[ValidateModel]
	public async Task<OperationResult> SubmitIelts([FromBody] SubmitIeltsModel model)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		model.UserId = userId;
		return await _service.SubmitIeltsAsync(model);
	}

	[HttpDelete("{id}")]
	public async Task<OperationResult> Delete(Guid id)
		=> await _service.DeleteAsync(id);
}