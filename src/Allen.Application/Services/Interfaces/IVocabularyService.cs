namespace Allen.Application;
public interface IVocabularyService
{
    Task<QueryResult<VocabulariesModel>> GetVocabulariesAsync(QueryInfo queryInfo);
    Task<VocabularyModel> GetVocabularyByIdAsync(Guid vocabId);
    Task<QueryResult<VocabularyModel>> GetVocabulariesByTopicIdAsync(Guid topic, QueryInfo queryInfo);
    Task<VocabularyModel> GetVocabularyByWordAsync(string word);
    Task<OperationResult> CreateAsync(CreateVocabularyModel vocabularyModel);
    Task<OperationResult> DeleteAsync(Guid vocabId);
    Task<OperationResult> UpdateAsync(UpdateVocabularyModel updateVocabularyModel, Guid vocabId);
    ///======================== Basic Functions ========================///
    Task<List<VocabularyEntity>> GetVocabulariesByIdsAsync(List<Guid> vocabIds);
    ///======================== Advanced Functions ========================///
    Task<QuizVocabulariesResponeModel> GetQuizVocabulariesAsync(QuizVocabulariesRequestModel model);
}
