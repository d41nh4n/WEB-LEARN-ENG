namespace Allen.Application;

public interface ILearningUnitsService
{
    Task<QueryResult<LearningUnit>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<LearningUnit>> GetByCategoryIdAsync(Guid id, QueryInfo queryInfo);
    Task<QueryResult<LearningUnit>> GetByFiltersAsync(LearningUnitQuery learningUnitQuery, QueryInfo queryInfo);
    Task<LearningUnit> GetByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateOrUpdateLearningUnitModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateLearningUnitModel model);
    Task<OperationResult> UpdateUnitStatusAsync(Guid id, UpdateLearningUnitStatusModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
