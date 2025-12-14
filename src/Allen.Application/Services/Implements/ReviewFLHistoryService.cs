

namespace Allen.Application;

[RegisterService(typeof(IReviewFLHistoryService))]
public class ReviewFLHistoryService
    (IReviewFLHistoryRepository _reviewFLHistoryRepository,
    IUnitOfWork _unitOfWork) : IReviewFLHistoryService
{
    public Task<int> GetReviewHistoryByUserIdToday(Guid userId)
    => _reviewFLHistoryRepository.GetReviewHistoryByUserIdToday(userId);
 

    public async Task SaveReviewHistorySnapshotAsync(
            Guid flashCardStateId,
            RatingLearningCard rating,
            double stability,
            double difficulty,
            int interval,
            int repetition)
    {
        // 1. Tạo Entity lịch sử (Snapshot) từ các tham số truyền vào
        var reviewHistory = new ReviewFLHistoryEntity
        {
            Id = Guid.NewGuid(),
            FlashCardStateId = flashCardStateId,
            ReviewDate = DateTime.UtcNow,
            Rating = rating,
            StabilityAtReview = stability,
            DifficultyAtReview = difficulty,
            IntervalAtReview = interval,
            RepetitionAtReview = repetition
        };
        await _unitOfWork.Repository<ReviewFLHistoryEntity>().AddAsync(reviewHistory);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(ReviewFLHistoryEntity));
        }
    }
}
