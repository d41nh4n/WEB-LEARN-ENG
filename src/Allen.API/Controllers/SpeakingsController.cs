namespace Allen.API.Controllers;

[Route("speakings")]
[Authorize]
public class SpeakingsController(
	ISpeakingsService _service,
	IBlobStorageService _blobStorageService,
	IAzureSpeechsService _azureSpeechsService) : BaseApiController
{
	//[HttpGet]
	//public async Task<QueryResult<SpeakingModel>> GetAll([FromQuery] SpeakingQuery query, [FromQuery] QueryInfo queryInfo)
	//{
	//    return await _service.GetAllAsync(query, queryInfo);
	//}

	[HttpGet("{learningUnitId}/learningunit")]
	public async Task<SpeakingModel> GetByLearningUnitId([FromRoute] Guid learningUnitId)
	{
		return await _service.GetByLearningUnitIdAsync(learningUnitId);
	}
	[HttpGet("ielts/{learningUnitId}/learningunit")]
	[AllowAnonymous]
	public async Task<List<SpeakingForIeltsModel>> GetByLearningUnitIdForIelts([FromRoute] Guid learningUnitId, [FromQuery] GetByLearningUnitIdForIeltsQuery query)
	  => await _service.GetByLearningUnitIdForIeltsAsync(learningUnitId, query);

	[HttpPost("learning")]
	[ValidateModel]
	public async Task<OperationResult> CreateLearning([FromBody] CreateSpeakingModel model)
	{
		return await _service.CreateLearningAsync(model);
	}

	[HttpPatch("learning/{id}")]
	[ValidateModel]
	public async Task<OperationResult> UpdateLearning([FromRoute] Guid id, [FromBody] UpdateSpeakingModel model)
	{
		return await _service.UpdateLearningAsync(id, model);
	}
	[HttpPost("ielts")]
	[ValidateModel]
	public async Task<OperationResult> CreateIelts([FromBody] CreateOrUpdateSpeakingIeltsModel model)
	{
		return await _service.CreateIeltsAsync(model);
	}

	[HttpPatch("ielts/{id}")]
	[ValidateModel]
	public async Task<OperationResult> UpdateIelts([FromRoute] Guid id, [FromBody] CreateOrUpdateSpeakingIeltsModel model)
	{
		return await _service.UpdateIeltsAsync(id, model);
	}

	[HttpPost("submit")]
	[Authorize(Roles = "User")]
	[ValidateModel]
	public async Task<OperationResult> Submit([FromBody] SubmitSpeakingModel model)
	{
		return await _service.SubmitAsync(model);
	}

	[HttpPost("submit-ielts")]
	[Authorize(Roles = "User")]
	[ValidateModel]
	public async Task<OperationResult> SubmitIelts([FromForm] SubmitSpeakingIeltsModel model)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _service.SubmitIeltsAsync(userId, model);
	}
	[HttpPost("analyze")]
	[Consumes("multipart/form-data")]
	[ValidateModel]
	public async Task<PronunciationAnalysisResultModel> AnalyzePronunciation([FromForm] PronunciationModel model)
	{
		return await _azureSpeechsService.AnalyzePronunciationAsync(model);
	}

	[HttpPost("transcribe")]
	[ValidateModel]
	public async Task<List<TranscribeResponseModel>> Transcribe([FromForm] TranscribeRequestModel model)
	{
		return await _azureSpeechsService.Transcribe(model);
	}
	[HttpPost("transcribe/stream")]
	public async Task TranscribeStream([FromForm] TranscribeRequestModel model)
	{
		Response.ContentType = "application/x-ndjson";
		await using var writer = new StreamWriter(Response.Body);
		await _azureSpeechsService.TranscribeStreamAsync(model, writer);
	}

	[HttpPost("upload-file")]
	public async Task<IActionResult> UploadFile([FromForm] TranscribeRequestModel file)
	{
		return Ok(await _blobStorageService.UploadFileAsync(AppConstants.AzureBlobStorage, file.File));
	}
	[HttpPost("delete-file")]
	public async Task<IActionResult> DeleteFile([FromBody] string url)
	{
		return Ok(await _blobStorageService.DeleteFileByUrlAsync(url));
	}
}
