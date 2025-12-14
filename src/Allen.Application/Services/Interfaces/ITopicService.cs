namespace Allen.Application;

public interface ITopicService
{
	Task<QueryResult<TopicModel?>> GetTopicsAsync(QueryInfo queryInfo);
	Task<TopicModel?> GetByIdAsync(Guid id);
	Task<OperationResult> CreateAsync(CreateTopicModel model);
	Task<OperationResult> UpdateAsync(Guid id, UpdateTopicModel model);
	Task<OperationResult> DeleteAsync(Guid id);
}
 