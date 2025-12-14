namespace Allen.Infrastructure;

public interface IRepositoryBase<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);
    void UpdateAsync(TEntity entity);
    Task DeleteByIdAsync(Guid id);
    void DeleteRangeAsync(IEnumerable<TEntity> entities);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
	Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition);
	Task<TEntity> GetByIdAsync(Guid id);
	Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, object>>[]? includes = null, CancellationToken cancellationToken = default);
	Task<TEntity?> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> GetListByConditionAsync(Expression<Func<TEntity, bool>> predicate);
	Task<IEnumerable<TEntity>> GetByIdsAsync(List<Guid> ids, string fieldsName);
	Task<QueryResult<TEntity>> GetByPageAsync(QueryInfo queryInfo);
    Task<TEntity?> GetAsync(Guid id, string? includeProperties = null);
    Task<bool> CheckExistByIdAsync(Guid id);
    Task<bool> CheckExistAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetQueryable();

}