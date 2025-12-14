namespace Allen.Application;

public interface IReadingPassagesService
{
    Task<QueryResult<ReadingPassageModel>> GetByUnitIdAsync(Guid unitId, QueryInfo queryInfo);
    Task<ReadingPassageModel> GetByIdAsync(Guid id);
    Task<ReadingPassageForLearningModel> GetLearningByIdAsync(Guid id);
    Task<List<ReadingPassageForIeltsModel>> GetLearningByIdForIeltsAsync(Guid id, ReadingPassageQuery query);
    Task<ReadingPassageSumaryModel> GetSumaryByUnitIdAsync(Guid learningUnitId);
    Task<OperationResult> CreateIeltsReadingPassageAsync(CreateIeltsReadingPassageModel model);
    Task<OperationResult> CreateIeltsReadingPassagesAsync(CreateIeltsReadingPassagesModel model);
    Task<OperationResult> CreateLearningReadingAsync(CreateLearningReadingPassageModel model);
    Task<OperationResult> SubmitIeltsReadingAsync(SubmitIeltsModel model);
    Task<OperationResult> UpdateAsync(Guid id, UpdateReadingPassagesModel model);
    Task<OperationResult> UpdateLearningAsync(Guid id, UpdateReadingPassageForLearningModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}