namespace Allen.Infrastructure;

public interface IVocabularyTagRepository : IRepositoryBase<VocabularyTagEntity>
{
    Task<IEnumerable<VocabularyTagEntity>> GetVocabularyTagByVocabIdAsync(Guid vocabularyId);
    Task<bool> CheckExistedByVocabularyId(Guid vocabularyId);
    Task<IEnumerable<Guid>> GetTagsIdByVocabularyIdAsync(Guid vocabularyId);
}
