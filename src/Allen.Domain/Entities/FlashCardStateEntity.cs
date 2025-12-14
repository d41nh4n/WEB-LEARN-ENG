namespace Allen.Domain;
[Table("CardStates")]
public class FlashCardStateEntity: EntityBase<Guid>
{
    public Guid FlashCardId { get; set; }

    // Fsrs fields--------------------------------

    /// <summary>
    /// S (Stability): hệ số ổn định.
    /// Mặc định = 0 (thẻ mới).
    /// Nằm trong khoảng từ 0.0 trở lên.
    /// </summary>
    public double Stability { get; set; } = 0;

    /// <summary>
    /// D (Difficulty): Độ khó cố hữu của thẻ (1-10).
    /// Mặc định = 5.0 (trung bình), hoặc 0 để logic tự gán.
    /// Dễ nhất (0) - Khó nhất (10).
    /// </summary>
    public double Difficulty { get; set; } = 5.0;

    /// <summary>
    /// R (Repetition): Số lần ôn tập .
    /// Măc định = 0 (chưa ôn tập).
    /// Tính tất cả các thao tác nhấn để phân biệt thẻ mới và thẻ cũ (đã học).
    /// </summary>
    public int Repetition { get; set; } = 0;

    /// <summary>
    /// I (Interval): Khoảng cách số ngày được lập lịch tính đến lần ôn tập tiếp theo.
    /// Mặc định = 0 (thẻ mới).
    /// Nằm trong khoảng từ 1 trở lên.
    /// </summary>
    public int Interval { get; set; } = 0;
    public DateTime? NextReviewDate { get; set; } = DateTime.UtcNow;

    // Audit fields-------------------------------
    public DateTime? LastReviewedAt { get; set; }
    public RatingLearningCard? LastRating { get; set; }

    public FlashCardEntity FlashCard { get; set; } = null!;
    public ICollection<ReviewFLHistoryEntity> ReviewHistories { get; set; } = [];

    public static FlashCardStateEntity CreateNew(Guid flashCardId)
    {
        return new FlashCardStateEntity
        {
            Id = Guid.NewGuid(),
            FlashCardId = flashCardId,
            Stability = 0,
            Difficulty = 0,
            Repetition = 0,
            Interval = 0,
            NextReviewDate = null,
            LastReviewedAt = null,
            LastRating = null
        };
    }
}
    