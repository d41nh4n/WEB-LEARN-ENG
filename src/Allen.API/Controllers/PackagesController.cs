namespace Allen.API.Controllers;

[Route("packages")]
public class PackagesController(IPackageService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<PackageModel>> GetPackages([FromQuery] QueryInfo queryInfo, [FromQuery] PackageQuery query)
        => await _service.GetPackagesAsync(queryInfo, query);

    [HttpGet("{id}")]
    public async Task<PackageModel> GetPackageById([FromRoute] Guid id)
        => await _service.GetPackageByIdAsync(id);

    [HttpPost]
    [ValidateModel]
    [Authorize(Roles = "Admin")]
    public async Task<OperationResult> Create([FromBody] CreatePackageModel model)
        => await _service.CreateAsync(model);

    [HttpPatch("{id}")]
    [ValidateModel]
    [Authorize(Roles = "Admin")]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] UpdatePackageModel model)
        => await _service.UpdateAsync(id, model);

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
        => await _service.DeleteAsync(id);
}
