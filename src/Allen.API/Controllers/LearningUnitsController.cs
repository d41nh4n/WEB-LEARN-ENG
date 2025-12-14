namespace Allen.API.Controllers;

[Route("learningunits")]
public class LearningUnitsController(ILearningUnitsService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<LearningUnit>> GetAllWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetAllWithPagingAsync(queryInfo);
    }
    [HttpGet("{categoryId}/category")]
    public async Task<QueryResult<LearningUnit>> GetByCategoryId(Guid categoryId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetByCategoryIdAsync(categoryId, queryInfo);
    }

    [HttpGet("skilltype")]
    [ValidateModel]
    public async Task<QueryResult<LearningUnit>> GetBySkillType([FromQuery] LearningUnitQuery learningUnitQuery, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetByFiltersAsync(learningUnitQuery, queryInfo);
    }

    [HttpGet("{id}")]
    public async Task<LearningUnit> GetById(Guid id)
    {
        return await _service.GetByIdAsync(id);
    }
 
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create(CreateOrUpdateLearningUnitModel model)
    {
        return await _service.CreateAsync(model);
    }
	[HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update(Guid id, CreateOrUpdateLearningUnitModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
	[HttpPatch("{id}/unit-status")]
	[ValidateModel]
	public async Task<OperationResult> UpdateUnitStatus(Guid id, UpdateLearningUnitStatusModel model)
	{
		return await _service.UpdateUnitStatusAsync(id, model);
	}
	[HttpDelete("{id}")]
    public async Task<OperationResult> Delete(Guid id)
    {
        return await _service.DeleteAsync(id);
    }
}
