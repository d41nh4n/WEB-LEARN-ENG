namespace Allen.Infrastructure;
public interface IUserVocabularyRepository
{
    Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserAsync(QueryInfo queryInfo, Guid userId);

    Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserByTopicIdAsync(QueryInfo queryInfo, Guid userId, Guid topicId);
}

