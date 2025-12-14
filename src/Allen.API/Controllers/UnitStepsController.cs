namespace Allen.API.Controllers;

[Route("unitsteps")]
[Authorize]
public class UnitStepsController(IUnitStepsService _service) : BaseApiController
{
    [HttpGet("{unitId}")]
    public async Task<QueryResult<UnitStep>> GetUnitStepsOfUnitWithPaging([FromRoute] Guid unitId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetUnitStepsOfUnitWithPagingAsync(unitId, queryInfo);
    }
    [HttpGet("step/{id}")]
    public async Task<UnitStep> GetById(Guid id)
    {
        return await _service.GetByIdAsync(id);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateOrUpdateUnitStepModel model)
    {
        return await _service.CreateAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] CreateOrUpdateUnitStepModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteAsync(id);
    }
}
