namespace Allen.API.Controllers;

[Route("vocabularies")]

public class VocabularyController(
    IVocabularyService _vocabularyService,
    IUserVocabularyService _userVocabularyService,
    IMeiliSearchService<VocabularyMLSModel> _meiliSearchService
) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<VocabulariesModel>> GetVocabularies([FromQuery] QueryInfo queryInfo)
        => await _vocabularyService.GetVocabulariesAsync(queryInfo);

    [HttpGet("{id}")]
    public async Task<VocabularyModel> GetById(Guid id)
    {
        return await _vocabularyService.GetVocabularyByIdAsync(id);
    }

    [HttpGet("{wordSearch}/word")]
    public async Task<VocabularyModel> GetByWord(string wordSearch)
    {
        return await _vocabularyService.GetVocabularyByWordAsync(wordSearch);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] AddMultipleVocabularyModel vocabularyModel)
        => await _vocabularyService.CreateMultiAsync(vocabularyModel);

    [HttpPatch("{id}")]
    [ValidateModel]
    [Authorize]
    public async Task<OperationResult> Update([FromBody] UpdateVocabularyModel vocabularyModel, Guid id)
    {
        return await _vocabularyService.UpdateAsync(vocabularyModel, id);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<OperationResult> Delete(Guid id)
        => await _vocabularyService.DeleteAsync(id);

    [HttpGet("dictionary-searching/{word}")]
    public async Task<List<VocabularyMLSModel>> SearchVocabulary(string word)
    {
        return await _meiliSearchService.SearchAsync(word);
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUser(
            [FromQuery] QueryInfo queryInfo)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _userVocabularyService.GetVocabulariesOfUserAsync(queryInfo, userId);
    }

    [HttpGet("topic/{topicId}")]
    public async Task<QueryResult<VocabularyModel>> GetVocabulariesOfUserByTopic(
        [FromQuery] QueryInfo queryInfo,
        [FromRoute] Guid topicId)
    {
        return await _vocabularyService.GetVocabulariesByTopicIdAsync(topicId, queryInfo);
    }

    [HttpPost("user/{vocabularyId}")]
    [Authorize]
    public async Task<OperationResult> AddVocabularyToUser(
        [FromRoute] Guid vocabularyId)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _userVocabularyService.AddVocabularyToUserAsync(userId, vocabularyId);
    }

    // 🔹 DELETE: api/users/{userId}/vocabularies/{vocabularyId}
    [HttpDelete("user/{vocabularyId}")]
    [Authorize]
    public async Task<OperationResult> RemoveVocabularyFromUser(
        [FromRoute] Guid vocabularyId)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _userVocabularyService.RemoveVocabularyFromUserAsync(userId, vocabularyId);
    }

    ///======================== Advanced Functions ========================///
    [HttpPost("quiz")]
    [Authorize]
    public async Task<QuizVocabulariesResponeModel> GetQuizVocabulariesAsync([FromBody]QuizVocabulariesRequestModel model)
    {
        return await _vocabularyService.GetQuizVocabulariesAsync(model);
    }

    [HttpPost("generate-vocabulary")]
    [ValidateModel]
    public async Task<VocabularyGenerateResponseModel> BuildVocabularyByWordsAsync(WordVocabularyForGenerateModel request)
    {
        return await _vocabularyService.BuildVocabularyByWordsAsync(request);
    }
}