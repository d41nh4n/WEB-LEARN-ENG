namespace Allen.Infrastructure;

public interface IReadingPassagesRepository : IRepositoryBase<ReadingPassageEntity>
{
    Task<ReadingPassageEntity> GetByIdForUpdate(Guid id);
    Task<QueryResult<ReadingPassageModel>> GetByUnitIdAsync(Guid unitId, QueryInfo queryInfo);
    Task<ReadingPassageForLearningModel> GetLearningByIdAsync(Guid id);
    Task<List<ReadingPassageForIeltsModel>> GetLearningByIdForIeltsAsync(Guid id, ReadingPassageQuery query);
    Task<ReadingPassageModel> GetQuestionByIdAsync(Guid id);
    Task<ReadingPassageSumaryModel> GetSumaryByUnitIdAsync(Guid learningUnitId);
}