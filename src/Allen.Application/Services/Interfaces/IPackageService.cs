namespace Allen.Application;

public interface IPackageService
{
    Task<QueryResult<PackageModel>> GetPackagesAsync(QueryInfo queryInfo, PackageQuery query);
    Task<PackageModel> GetPackageByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreatePackageModel model);
    Task<OperationResult> UpdateAsync(Guid id, UpdatePackageModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
