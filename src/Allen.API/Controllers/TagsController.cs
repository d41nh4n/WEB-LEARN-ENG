using Allen.API.Controllers;

[Route("tags")]
public class TagsController(
    ITagService _tagService
) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<TagModel?>> GetTags([FromQuery] QueryInfo queryInfo) => await _tagService.GetTagsAsync(queryInfo);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(Guid id)
    {
        var tag = await _tagService.GetByIdAsync(id);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> CreateTag([FromBody] CreateTagModel model)
        => await _tagService.CreateAsync(model);

    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateTag(Guid id, [FromBody] UpdateTagModel model)
    {
        return await _tagService.UpdateAsync(model, id);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> DeleteTag(Guid id)
        => await _tagService.DeleteAsync(id);
}
