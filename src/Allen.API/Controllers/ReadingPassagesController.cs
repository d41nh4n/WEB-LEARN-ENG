namespace Allen.API.Controllers;

[Route("readingpassages")]
[Authorize]
public class ReadingPassagesController(IReadingPassagesService _service) : BaseApiController
{
    [HttpGet("learning/{id}/learningunit")]
    [AllowAnonymous]
    public async Task<ReadingPassageForLearningModel> GetLearningById([FromRoute] Guid id)
    {
        return await _service.GetLearningByIdAsync(id);
    }
    [HttpGet("ielts/{id}/learningunit")]
    [AllowAnonymous]
    [ValidateModel]
    public async Task<List<ReadingPassageForIeltsModel>> GetLearningByIdForIelts([FromRoute] Guid id, [FromQuery] ReadingPassageQuery query)
    {
        return await _service.GetLearningByIdForIeltsAsync(id, query);
    }
    [HttpGet("{learningUnitId}/learning-unit/summary")]
    [AllowAnonymous]
    public async Task<ReadingPassageSumaryModel> GetSumaryByUnitId([FromRoute] Guid learningUnitId)
	{
		return await _service.GetSumaryByUnitIdAsync(learningUnitId);
	}
	[HttpPost("learning")]
	[ValidateModel]
	public async Task<OperationResult> CreateLearningReading([FromBody] CreateLearningReadingPassageModel model)
		=> await _service.CreateLearningReadingAsync(model);

    [HttpPost("ielts")]
    [ValidateModel]
    public async Task<OperationResult> CreateIeltsReadingPassage([FromBody] CreateIeltsReadingPassageModel model)
        => await _service.CreateIeltsReadingPassageAsync(model);


    [HttpPost("ielts-bulk")]
    [ValidateModel]
    public async Task<OperationResult> CreateIeltsReading([FromBody] CreateIeltsReadingPassagesModel model)
        => await _service.CreateIeltsReadingPassagesAsync(model);


    [HttpPost("ielts/reading/submit")]
	[Authorize(Roles = "User")]
	[ValidateModel]
    public async Task<OperationResult> SubmitIeltsReading([FromBody] SubmitIeltsModel model)
    {
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        model.UserId = userId;
        return await _service.SubmitIeltsReadingAsync(model);
    } 

    [HttpPatch("ielts/{id}")]
	[ValidateModel]
	public async Task<OperationResult> Update(Guid id, [FromBody] UpdateReadingPassagesModel model)
	{
		return await _service.UpdateAsync(id, model);
	}
    [HttpPatch("learning/{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateLearning(Guid id, [FromBody] UpdateReadingPassageForLearningModel model)
    {
        return await _service.UpdateLearningAsync(id, model);
    }

    [HttpDelete("{id}")]
	public async Task<OperationResult> Delete(Guid id)
	{
		return await _service.DeleteAsync(id);
	}
}
