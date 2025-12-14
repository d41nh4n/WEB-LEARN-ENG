namespace Allen.Domain;

public class FlashCardStatisticsModel
{
    /// <summary>
    /// Tổng số Flashcard được tạo ra từ Vocabulary Entity (có RelatonVocabularyCardId != null)
    /// </summary>
    public int TotalCardsLinkedToVocabulary { get; set; }

    /// <summary>
    /// Tổng số Flashcard đã từng được ôn tập (Repetition > 0)
    /// </summary>
    public int TotalLearnedCards { get; set; }

    /// <summary>
    /// Tổng số Flashcard cần ôn tập trong ngày hôm nay (NextReviewDate <= Today)
    /// </summary>
    public int TotalCardsDueToday { get; set; }

    /// <summary>
    /// Tổng số Flashcard ĐÃ HỌC và KHÔNG CẦN ôn tập hôm nay (NextReviewDate > Today)
    /// </summary>
    public int TotalCardsScheduledForFutureReview { get; set; }

    /// <summary>
    /// Tổng số thẻ đã học/ôn tập trong 24 giờ qua (Cần lấy từ ReviewHistory)
    /// </summary>
    public int TotalCardsReviewedToday { get; set; }
}
