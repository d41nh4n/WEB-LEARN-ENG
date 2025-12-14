namespace Allen.Application;

public interface IWritingService
{
    // -- Learning --
    Task<WritingLearningModel> GetLearningWritingByIdAsync(Guid id);
    Task<WritingLearningModel> GetLearningWritingByLearningIdAsync(Guid id);
    Task<QueryResult<WritingLearningModel>> GetLearningWritingsAsync(QueryInfo queryInfo);
    Task<OperationResult> CreateLearningAsync(CreateLearningWritingModel model);
    Task<OperationResult> UpdateLearningAsync(Guid id, UpdateLearningWritingModel model);

    // -- Ielts --
    Task<WritingIeltsModel> GetIeltsWritingByIdAsync(Guid id);
    Task<WritingIeltsModel> GetIeltsWritingByLearningIdAsync(Guid id);
    Task<QueryResult<WritingIeltsModel>> GetIeltsWritingsAsync(QueryInfo queryInfo);
    Task<OperationResult> CreateIeltsAsync(CreateIeltsWritingModel model);
    Task<OperationResult> UpdateIeltsAsync(Guid id, UpdateIeltsWritingModel model);

    // -- Common --
    Task<OperationResult> DeleteAsync(Guid id);
}
