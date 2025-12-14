namespace Allen.API.Controllers;

[Route("topics")]
public class TopicsController(ITopicService _service) : BaseApiController
{
	[HttpGet]
	public async Task<QueryResult<TopicModel?>> GetTopics([FromQuery] QueryInfo queryInfo)
	{
		return await _service.GetTopicsAsync(queryInfo);
	}
	[HttpGet("{id}")]
	public async Task<TopicModel?> GetById([FromRoute] Guid id)
	{
		return await _service.GetByIdAsync(id);
	}
	[HttpPost]
	[ValidateModel]
	public async Task<OperationResult> CreateTopic([FromBody] CreateTopicModel model)
	{
		return await _service.CreateAsync(model);
	}
	[HttpPatch("{id}")]
	[ValidateModel]
	public async Task<OperationResult> UpdateTopic(Guid id, [FromBody] UpdateTopicModel model)
	{
		return await _service.UpdateAsync(id, model);
	}
	[HttpDelete("{id}")]
	[ValidateModel]
	public async Task<OperationResult> DeleteTopic(Guid id)
	{
		return await _service.DeleteAsync(id);
	}
}
