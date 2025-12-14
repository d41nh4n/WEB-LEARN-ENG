namespace Allen.API.Controllers;

[Route("categories")]
public class CategoriesController(ICategoriesService _service) : BaseApiController
{
    [HttpGet]
    [ValidateModel]
    public async Task<QueryResult<CategoryModel>> GetCategories([FromQuery] CategoryQuery categoryQuery)
    {
        return await _service.GetCategoriesAsync(categoryQuery);
    }
    [HttpGet("{id}")]
    public async Task<CategoryModel> GetById([FromRoute] Guid id)
    {
        return await _service.GetByIdAsync(id);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateOrUpdateCategoryModel model)
    {
        return await _service.CreateAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update(Guid id, [FromBody] CreateOrUpdateCategoryModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete(Guid id)
    {
        return await _service.DeleteAsync(id);
    }
}
