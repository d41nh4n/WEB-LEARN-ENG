namespace Allen.Infrastructure;

public interface IPackageRepository : IRepositoryBase<PackageEntity>
{
    Task<QueryResult<PackageModel>> GetPackagesAsync(QueryInfo queryInfo, PackageQuery query);
    Task<PackageEntity?> FindAsync(Expression<Func<PackageEntity, bool>> predicate);
}
