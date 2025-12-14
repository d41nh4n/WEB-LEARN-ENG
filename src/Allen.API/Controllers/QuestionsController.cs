namespace Allen.API.Controllers;

[Route("questions")]
public class QuestionsController(IQuestionsService _service) : BaseApiController
{
    [HttpGet("{moduleItemId}/learningunit")]
    public async Task<QueryResult<QuestionModel>> GetQuestions([FromRoute] Guid moduleItemId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetQuestionsAsync(moduleItemId, queryInfo);
    }
    [HttpGet("{id}")]
    public async Task<QuestionModel> GetById(Guid id)
    {
        return await _service.GetQuestionByIdAsync(id);
    }

    [HttpGet("answer/{learningUnitId}/learningunit")]
    [Authorize]
	public async Task<List<AnswerResult>> GetAnswersOfUserByLearningId(Guid learningUnitId)
    {
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        return await _service.GetAnswersOfUserByLearningIdAsync(learningUnitId, userId);
	}
	[HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromForm] CreateOrUpdateQuestionModel model)
    {
        return await _service.CreateQuestionAsync(model);
    }
    [HttpPost("ielts/reading")]
    [ValidateModel]
    public async Task<OperationResult> CreateQuestionForReading([FromBody] CreateOrUpdateQuestionForReadingModel model)
    {
        return await _service.CreateQuestionForReadingAsync(model);
    }
    [HttpPost("ielts/listening")]
    [ValidateModel]
    public async Task<OperationResult> CreateQuestionForListening([FromBody] CreateOrUpdateQuestionForListeningModel model)
    {
        return await _service.CreateQuestionForListeningAsync(model);
    }
	[HttpPost("ielts/speaking")]
	[ValidateModel]
	public async Task<OperationResult> CreateQuestionForSpeaking([FromBody] CreateOrUpdateQuestionForSpeakingModel model)
	{
		return await _service.CreateQuestionForSpeakingAsync(model);
	}
	[HttpPost("bulk")]
    [ValidateModel]
    public async Task<OperationResult> CreateQuestionsAsync([FromForm] List<CreateOrUpdateQuestionModel> models)
    {
        return await _service.CreateQuestionsAsync(models);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromForm] CreateOrUpdateQuestionModel model)
    {
        return await _service.UpdateQuestionAsync(id, model);
    }
    [HttpPatch("ielts/reading/{id}")]
    //[ValidateModel]
    public async Task<OperationResult> UpdateQuestionForReading([FromRoute] Guid id, [FromBody] CreateOrUpdateQuestionForReadingModel model)
    {
        return await _service.UpdateQuestionForReadingAsync(id, model);
    }
    [HttpPatch("ielts/listening/{id}")]
    //[ValidateModel]
    public async Task<OperationResult> UpdateForListening([FromRoute] Guid id, [FromBody] CreateOrUpdateQuestionForListeningModel model)
    {
        return await _service.UpdateQuestionForListeningAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteQuestionAsync(id);
    }
}
