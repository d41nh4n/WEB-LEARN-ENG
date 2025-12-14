namespace Allen.Application;

public interface IUnitStepsService
{
    Task<QueryResult<UnitStep>> GetUnitStepsOfUnitWithPagingAsync(Guid unitId, QueryInfo queryInfo);
    Task<UnitStep> GetByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateOrUpdateUnitStepModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateUnitStepModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
