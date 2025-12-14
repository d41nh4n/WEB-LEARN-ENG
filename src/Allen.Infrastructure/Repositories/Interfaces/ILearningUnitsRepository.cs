namespace Allen.Infrastructure;

public interface ILearningUnitsRepository : IRepositoryBase<LearningUnitEntity>
{
    Task<QueryResult<LearningUnit>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<LearningUnit>> GetByCategoryIdAsync(Guid categoryId, QueryInfo queryInfo);
    Task<QueryResult<LearningUnit>> GetByFiltersAsync(LearningUnitQuery learningUnitQuery, QueryInfo queryInfo);
    Task<LearningUnit> GetLearningUnitByIdAsync(Guid id);
}
