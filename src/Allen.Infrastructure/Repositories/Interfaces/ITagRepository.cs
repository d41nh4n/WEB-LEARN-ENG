namespace Allen.Infrastructure;

public interface ITagRepository : IRepositoryBase<TagEntity>
{
    Task<QueryResult<TagModel?>> GetTagsAsync(QueryInfo queryInfo);

    Task<string?> GetNameTagByIdsAsync(Guid id);

    Task<int> CountExistingTagsAsync(List<Guid> tagsId);

    Task<IEnumerable<Guid>> GetAllTagsAsync(List<Guid> tagsId);
}