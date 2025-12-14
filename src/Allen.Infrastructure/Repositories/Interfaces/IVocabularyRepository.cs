namespace Allen.Infrastructure;
public interface IVocabularyRepository : IRepositoryBase<VocabularyEntity>
{
    Task<QueryResult<VocabulariesModel>> GetVocabulariesAsync(QueryInfo queryInfo);

    Task<VocabularyModel> GetVocabularyByIdAsync(Guid vocabId);

    Task<VocabularyModel> GetVocabularyByWordAsync(string word);

    Task<QuizVocabulariesResponeModel> GetQuizVocabulariesAsync(QuizVocabulariesRequestModel model);

    Task<List<VocabularyEntity>> GetVocabulariesByIdsAsync(List<Guid> vocabIds);

    Task<QueryResult<VocabularyModel>> GetVocabulariesByTopicIdAsync(QueryInfo queryInfo, Guid topic);

    Task<List<VocabularyEntity>> GetVocabulariesByWordsAsync(List<string> words);
}