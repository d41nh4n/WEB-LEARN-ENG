namespace Allen.Application;

public interface IFeedbacksService
{
	Task<QueryResult<FeedbackModel>> GetFeedbacksAsync(FeedbackQuery query, QueryInfo queryInfo);
	Task<QueryResult<FeedbackModel>> GetFeedbacksOfUserAsync(Guid userId, FeedbackQuery query, QueryInfo queryInfo);
	Task<FeedbackModel> GetByIdAsync(Guid id);
	Task<FeedbackModel> GetByIdOfUserAsync(Guid userId, Guid id);
	Task<OperationResult> CreateAsync(Guid userId, CreateOrUpdateFeedbackModel model);
	Task<OperationResult> UpdateAsync(Guid id, Guid userId, CreateOrUpdateFeedbackModel model);
	Task<OperationResult> DeleteAsync(Guid id);
}
