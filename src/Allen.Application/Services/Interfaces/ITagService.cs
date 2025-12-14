namespace Allen.Application;

public interface ITagService
{
    Task<OperationResult> CreateAsync(CreateTagModel model);
    Task<OperationResult> UpdateAsync(UpdateTagModel model, Guid id);
    Task<OperationResult> DeleteAsync(Guid id);
    Task<TagModel?> GetByIdAsync(Guid tagId);
    Task<QueryResult<TagModel?>> GetTagsAsync(QueryInfo queryInfo);
    Task<List<Guid>> CheckNotExistedTagAsync(List<Guid> tagsId);
}
