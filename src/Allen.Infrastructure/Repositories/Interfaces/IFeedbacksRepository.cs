namespace Allen.Infrastructure;

public interface IFeedbacksRepository : IRepositoryBase<FeedbackEntity>
{
	Task<QueryResult<FeedbackModel>> GetFeedbacksAsync(FeedbackQuery query, QueryInfo queryInfo);
	Task<QueryResult<FeedbackModel>> GetFeedbacksOfUserAsync(Guid userId, FeedbackQuery query, QueryInfo queryInfo);
	Task<FeedbackModel> GetFeedbackByIdAsync(Guid id);
	Task<FeedbackModel> GetByIdOfUserAsync(Guid userId, Guid id);

}
