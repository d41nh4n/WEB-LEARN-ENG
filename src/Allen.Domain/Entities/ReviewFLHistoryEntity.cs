namespace Allen.Domain;

[Table("ReviewFLHistories")]
public class ReviewFLHistoryEntity : EntityBase<Guid>
{
    /// <summary>
    /// Khóa ngoại tới trạng thái hiện tại của thẻ (FlashCardState)
    /// </summary>
    public Guid FlashCardStateId { get; set; }
    public FlashCardStateEntity FlashCardState { get; set; } = null!;

    // Thông tin review
    /// <summary>
    /// Thời điểm thẻ được review.
    /// </summary>
    public DateTime ReviewDate { get; set; }
    /// <summary>
    /// Đánh giá của người dùng về độ khó của thẻ (Again, Hard, Good, Easy).
    /// </summary>
    public RatingLearningCard Rating { get; set; }

    // Snapshot FSRS (Lưu lại trạng thái FSRS tại thời điểm review)
    /// <summary>
    /// Độ ổn định (Stability) của thẻ tại thời điểm review.
    /// </summary>
    public double StabilityAtReview { get; set; }
    /// <summary>
    /// Độ khó (Difficulty) của thẻ tại thời điểm review.
    /// </summary>
    public double DifficultyAtReview { get; set; }
    /// <summary>
    /// Khoảng thời gian (Interval) hiện tại của thẻ trước khi review.
    /// </summary>
    public int IntervalAtReview { get; set; }
    /// <summary>
    /// Số lần lặp lại (Repetition) của thẻ trước khi review.
    /// </summary>
    public int RepetitionAtReview { get; set; }
}