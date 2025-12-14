namespace Allen.Application;
public interface IReviewFLHistoryService
{
    /// <summary>
    /// Tạo và lưu một bản ghi lịch sử ôn tập mới, chụp lại trạng thái thẻ hiện tại.
    /// </summary>
    /// <param name="flashCardStateId">ID của trạng thái thẻ (khóa ngoại).</param>
    /// <param name="rating">Đánh giá của người dùng.</param>
    /// <param name="stability">Stability (S) trước review.</param>
    /// <param name="difficulty">Difficulty (D) trước review.</param>
    /// <param name="interval">Interval (I) trước review.</param>
    /// <param name="repetition">Repetition (R) trước review.</param>
    Task SaveReviewHistorySnapshotAsync(
        Guid flashCardStateId,
        RatingLearningCard rating,
        double stability,
        double difficulty,
        int interval,
        int repetition);

    Task<int> GetReviewHistoryByUserIdToday(Guid userId);
}
