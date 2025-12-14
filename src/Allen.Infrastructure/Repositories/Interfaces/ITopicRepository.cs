namespace Allen.Infrastructure;

public interface ITopicRepository : IRepositoryBase<TopicEntity>
{
	Task<QueryResult<TopicModel?>> GetTopicsAsync(QueryInfo queryInfo);
	Task<TopicModel?> GetTopicByIdAsync(Guid id);
}
