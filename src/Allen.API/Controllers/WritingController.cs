namespace Allen.API.Controllers;

[Route("writings")]
[Authorize]
public class WritingController(IWritingService _service, IWritingSubmissionService _submissionService) : BaseApiController
{
    #region Learning
    [HttpGet("learning/{id}")]
    [AllowAnonymous]
    public async Task<WritingLearningModel> GetLearningWritingById(Guid id)
        => await _service.GetLearningWritingByIdAsync(id);

    [HttpGet("learning/{id}/learningUnit")]
	[AllowAnonymous]
	public async Task<WritingLearningModel> GetLearningWritingByLearningUnitId(Guid id)
        => await _service.GetLearningWritingByLearningIdAsync(id);

    [HttpGet("learning")]
    public async Task<QueryResult<WritingLearningModel>> getalllearningwriting([FromQuery] QueryInfo queryinfo)
        => await _service.GetLearningWritingsAsync(queryinfo);

    [HttpPost("learning")]
    [ValidateModel]
    public async Task<OperationResult> CreateAcademy([FromBody] CreateLearningWritingModel model)
       => await _service.CreateLearningAsync(model);

    [HttpPatch("learning/{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateAcademy(Guid id, [FromBody] UpdateLearningWritingModel model)
        => await _service.UpdateLearningAsync(id, model);

    #endregion
    #region Ielts
    [HttpGet("ielts/{id}")]
	[AllowAnonymous]
	public async Task<WritingIeltsModel> GetIeltsById(Guid id)
        => await _service.GetIeltsWritingByIdAsync(id);

    [HttpGet("ielts/{id}/learningUnit")]
	[AllowAnonymous]
	public async Task<WritingIeltsModel> GetIeltsByLearningUnitId(Guid id)
        => await _service.GetIeltsWritingByLearningIdAsync(id);

    [HttpGet("ielts")]
	[AllowAnonymous]
	public async Task<QueryResult<WritingIeltsModel>> GetAllIelts([FromQuery] QueryInfo queryInfo)
        => await _service.GetIeltsWritingsAsync(queryInfo);

    [HttpPost("ielts")]
    [ValidateModel]
    public async Task<OperationResult> CreateIelts([FromForm] CreateIeltsWritingModel model)
       => await _service.CreateIeltsAsync(model);

    [HttpPatch("ielts/{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateIelts(Guid id, [FromForm] UpdateIeltsWritingModel model)
        => await _service.UpdateIeltsAsync(id, model);

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete(Guid id)
        => await _service.DeleteAsync(id);
    #endregion

    [HttpPost("submit-stream")]
    [ValidateModel]
    public async Task SubmitStream([FromBody] LearningWritingSubmitModel model)
    {
        var userId = HttpContext.GetCurrentUserId();
        model.UserId = userId;

        await foreach (var chunk in _submissionService.SubmitSentenceAsync(model))
        {
            await Response.WriteAsync(chunk);
            await Response.Body.FlushAsync();
        }
    }

    [HttpPost("submit-ielts")]
    [ValidateModel]
    public async Task<OperationResult> IeltsWritingSubmit([FromBody] IeltsWritingSubmitModel model)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _submissionService.IeltsWritingSubmitAsync(userId, model);
    }
}