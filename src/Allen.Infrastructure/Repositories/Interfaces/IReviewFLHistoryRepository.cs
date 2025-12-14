namespace Allen.Infrastructure;

public interface IReviewFLHistoryRepository
{
    Task<int> GetReviewHistoryByUserIdToday(Guid userId);
}
