using Allen.Common;

namespace Allen.Application;

public interface IFlashCardsService
{
    Task<FlashCardModel> GetByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateOrUpdateFlashCardWithFileModel model, Guid userId, Guid deckId);
    Task<OperationResult> CreateMultiAsync(FlashCardCreateMultiRequestModel model, Guid userId, Guid deckId);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateFlashCardWithFileModel model, Guid userId, Guid deckId);
    Task<OperationResult> DeleteAsync(Guid id, Guid userId, Guid deckId);

    // Logic mở rộng cho FlashCard
    Task<QueryResult<Guid>> GetFlashCardIdsByDeck(QueryInfo queryInfo, Guid deckId, Guid userId);
    Task<QueryResult<FlashCardStudyQueueLstModel>> GetStudyQueueAsync(Guid deckId, Guid userId);

    Task<CursorQueryResult<FlashCardStudyModel>> GetFlashCardsByDeckID(CursorQueryInfo queryInfo, Guid deckId, Guid userId);
    Task<OperationResult> ProcessReviewAsync(Guid deckId, Guid flashCardId, ReviewFlashCardRequestModel request, Guid userId);

    Task<List<Guid>> GetListUserIdToNotificateStudyToday();

    Task<Dictionary<Guid, int>> GetNumberNeedReviewTodayForUserIds(List<Guid> userIds);

    // ==================== LOGIC NÂNG CAO (DECK OPERATIONS) ====================

    /// <summary>
    /// Clone (Nhân bản) toàn bộ một Deck sang một Deck mới hoàn toàn.
    /// (Tạo Deck mới -> Copy tất cả Card -> Reset CardState về New)
    /// </summary>
    Task<OperationResult> CloneWholeDeckAsync(Guid sourceDeckId, Guid userId);

    /// <summary>
    /// Copy nội dung từ Deck nguồn và Paste vào Deck đích.
    /// (Lấy tất cả Card từ Source -> Tạo bản sao -> Thêm vào Target Deck)
    /// </summary>
    Task<OperationResult> ImportCardsFromDeckAsync(Guid sourceDeckId, Guid targetDeckId, Guid userId);

    /// <summary>
    /// Copy một danh sách các thẻ cụ thể sang Deck đích.
    /// </summary>
    Task<OperationResult> CopyFlashCardsToDeckAsync(FlashCardsToDeckRequestModel model, Guid targetDeckId, Guid userId);

    /// <summary>
    /// Di chuyển (Move) các thẻ từ Deck hiện tại sang Deck đích.
    /// (Thay đổi DeckId của thẻ, giữ nguyên hoặc reset tiến độ tuỳ logic)
    /// </summary>
    /// <param name="flashCardIds">Danh sách thẻ cần chuyển</param>
    /// <param name="targetDeckId">Deck đích</param>
    /// <param name="userId">Chủ sở hữu</param>
    Task<OperationResult> MoveFlashCardsToDeckAsync(FlashCardsToDeckRequestModel model, Guid targetDeckId, Guid userId);

    /// <summary>
    /// Đặt lại tiến độ học (Reset Progress) cho toàn bộ Deck hoặc một danh sách thẻ.
    /// (Đưa S, D, Repetition, Interval về mặc định như mới)
    /// </summary>
    /// <param name="deckId">ID của Deck cần reset</param>
    /// <param name="userId">Chủ sở hữu</param>
    Task<OperationResult> ResetProgressAsync(Guid deckId, Guid userId);

    /// <summary>
    /// Xóa nhiều thẻ cùng lúc (Batch Delete).
    /// </summary>
    /// <param name="flashCardIds">Danh sách ID thẻ cần xóa</param>
    /// <param name="userId">Chủ sở hữu</param>
    /// <param name="deckId">ID của Deck (để verify quyền)</param>
    Task<OperationResult> DeleteMultiAsync(DeleteFlashCardLstModel cardLstModel, Guid userId, Guid deckId);

    ///=================== LOGIC NÂNG CAO (Vocabulary) ====================
    /// <summary>
    /// Tao thẻ từ danh sách từ vựng
    /// </summary>
    /// <param name="vocabIds">Danh sách ID từ vựng</param>
    /// <param name="userId">Chủ sở hữu</param>
    /// <param name="deckId">ID của Deck (để verify quyền)</param>
    Task<OperationResult> CreateFromVocabularyAsync(CreateFlashCardByVocabularyModel vocabIds, Guid userId, Guid deckId);

    /// <summary>
    /// Lấy data để khởi tạo màn hình từ vựng
    /// </summary>
    /// <param name="userId">Chủ sở hữu</param>
    public Task<FlashCardStatisticsModel> GetUserFlashCardStatisticsAsync(Guid userId);
}
