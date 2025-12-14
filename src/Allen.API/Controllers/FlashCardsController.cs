using Allen;
using Allen.API.Controllers;

[Route("decks")]
public class DecksController(
    IFlashCardsService _flashCardService,
    IDeckService _deckService) : BaseApiController
{
    // ==================== DECK ENDPOINTS ====================
    [HttpGet("{id}")]
    public async Task<DeckModel?> GetDeckById([FromRoute] Guid id)
        => await _deckService.GetByIdAsync(id);

    [HttpGet]
    [Authorize]
    public async Task<List<DeckModel>> GetDeckByUserId()
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _deckService.GetByUserIdAsync(currentUserId);
    }

    [HttpPost]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> CreateDeck([FromBody] CreateDeckModel model)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _deckService.CreateAsync(model, currentUserId);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<OperationResult> DeleteDeckById([FromRoute] Guid id)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _deckService.DeleteByIdAsync(currentUserId, id);
    }

    [HttpPatch("{id}")]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> UpdateDeck([FromRoute] Guid id, [FromBody] UpdateDeckModel model)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _deckService.UpdateAsync(model, id, currentUserId);
    }

    // ==================== FLASHCARD ENDPOINTS ====================

    [HttpGet("{deckId}/flashcards")]
    [Authorize]
    public async Task<QueryResult<Guid>> GetFlashCardIdsByDeck([FromRoute] Guid deckId, [FromQuery] QueryInfo queryInfo)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.GetFlashCardIdsByDeck(queryInfo, deckId, currentUserId);
    }

    [HttpGet("flashcards/{id}")]
    [Authorize]
    public async Task<FlashCardModel> GetFlashCardById([FromRoute] Guid id)
        => await _flashCardService.GetByIdAsync(id);

    [HttpPost("{deckId}/flashcards")]
    [Authorize]
    [ValidateModel]
    [Consumes("multipart/form-data")]
    public async Task<OperationResult> CreateFlashCard([FromRoute] Guid deckId, [FromForm] CreateOrUpdateFlashCardWithFileModel model)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.CreateAsync(model, currentUserId, deckId);
    }

    [HttpPost("{deckId}/multi-flashcards")]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> CreateMultiFlashCard([FromRoute] Guid deckId, [FromBody] FlashCardCreateMultiRequestModel model)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.CreateMultiAsync(model, currentUserId, deckId);
    }

    [HttpPatch("{deckId}/flashcards/{id}")]
    [Authorize]
    [ValidateModel]
    [Consumes("multipart/form-data")]
    public async Task<OperationResult> UpdateFlashCard([FromRoute] Guid id, [FromForm] CreateOrUpdateFlashCardWithFileModel model, [FromRoute] Guid deckId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.UpdateAsync(id, model, currentUserId, deckId);
    }

    [HttpDelete("{deckId}/flashcards/{id}")]
    [Authorize]
    public async Task<OperationResult> DeleteFlashCard([FromRoute] Guid id, [FromRoute] Guid deckId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.DeleteAsync(id, currentUserId, deckId);
    }

    [HttpGet("{deckId}/flashcards/all-cards")]
    [Authorize]
    [ValidateModel]
    public async Task<CursorQueryResult<FlashCardStudyModel>> GetAllCardsQueueAsync([FromRoute] Guid deckId, [FromQuery] CursorQueryInfo cursorQueryInfo)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.GetFlashCardsByDeckID(cursorQueryInfo, deckId, currentUserId);
    }

    [HttpGet("{deckId}/study-queue")]
    [Authorize]
    public async Task<QueryResult<FlashCardStudyQueueLstModel>> GetStudyQueueAsync([FromRoute] Guid deckId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.GetStudyQueueAsync(deckId, currentUserId);
    }

    [HttpPost("{deckId}/flash-card/study-queue/{id}")]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> ProcessReviewAsync([FromBody] ReviewFlashCardRequestModel request, [FromRoute] Guid deckId, [FromRoute] Guid id)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.ProcessReviewAsync(deckId, id, request, currentUserId);
    }

    /// <summary>
    /// Clone (Sao chép) toàn bộ Deck này thành một Deck mới của người dùng hiện tại.
    /// </summary>
    [HttpPost("{sourceDeckId}/clone")]
    [Authorize]
    public async Task<OperationResult> CloneWholeDeck([FromRoute] Guid sourceDeckId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.CloneWholeDeckAsync(sourceDeckId, currentUserId);
    }

    /// <summary>
    /// Import (Nhập) tất cả thẻ từ một Deck nguồn vào Deck đích (targetDeckId).
    /// </summary>
    [HttpPost("{targetDeckId}/import")]
    [Authorize]
    public async Task<OperationResult> ImportCardsFromDeck([FromRoute] Guid targetDeckId, [FromBody] ImportCardsFromDeckRequestModel model)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.ImportCardsFromDeckAsync(model.SourceDeckId, targetDeckId, currentUserId);
    }

    /// <summary>
    /// Copy (Sao chép) một danh sách thẻ cụ thể vào Deck này.
    /// </summary>
    [HttpPost("{targetDeckId}/card-actions/copy")]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> CopyFlashCards([FromBody] FlashCardsToDeckRequestModel model, [FromRoute] Guid targetDeckId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.CopyFlashCardsToDeckAsync(model, targetDeckId, currentUserId);
    }

    /// <summary>
    /// Move (Di chuyển) một danh sách thẻ cụ thể sang Deck này.
    /// </summary>
    [HttpPost("{targetDeckId}/card-actions/move")]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> MoveFlashCards([FromBody] FlashCardsToDeckRequestModel model, [FromRoute] Guid targetDeckId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.MoveFlashCardsToDeckAsync(model, targetDeckId, currentUserId);
    }

    /// <summary>
    /// Reset tiến độ học (Về trạng thái New) cho toàn bộ Deck hoặc một số thẻ.
    /// </summary>
    [HttpPost("{deckId}/reset-progress")]
    [Authorize]
    public async Task<OperationResult> ResetProgress([FromRoute] Guid deckId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.ResetProgressAsync(deckId, currentUserId);
    }

    /// <summary>
    /// Xóa hàng loạt thẻ trong một Deck.
    /// </summary>
    [HttpDelete("{deckId}/flashcards/batch-delete")]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> DeleteMultiFlashCards([FromRoute] Guid deckId, [FromBody] DeleteFlashCardLstModel cardLstModel)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.DeleteMultiAsync(cardLstModel, currentUserId, deckId);
    }

    [HttpPost("{deckId}/flashcards/create-by-vocabularies")]
    [Authorize]
    [ValidateModel]
    public async Task<OperationResult> CreateFromVocabularyAsync([FromRoute] Guid deckId, [FromBody] CreateFlashCardByVocabularyModel model)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.CreateFromVocabularyAsync(model, currentUserId, deckId);
    }

    [HttpGet("flashcards/statistics")]
    [Authorize]
    public async Task<FlashCardStatisticsModel> GetUserFlashCardStatisticsAsync()
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        return await _flashCardService.GetUserFlashCardStatisticsAsync(currentUserId);
    }
}
