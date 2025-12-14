namespace Allen.Application;
public interface IUserVocabularyService
{
    Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserAsync(QueryInfo queryInfo, Guid userId);

    Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserByTopicIdAsync(QueryInfo queryInfo, Guid userId, Guid topicId);

    Task<OperationResult> AddVocabularyToUserAsync(Guid userId, Guid vocabularyId);

    Task<OperationResult> RemoveVocabularyFromUserAsync(Guid userId, Guid vocabularyId);
}

