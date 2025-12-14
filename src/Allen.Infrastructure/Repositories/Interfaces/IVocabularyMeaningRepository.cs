namespace Allen.Infrastructure;

public interface IVocabularyMeaningRepository : IRepositoryBase<VocabularyMeaningEntity>
{
    Task<IEnumerable<VocabularyMeaningEntity>> GetVocabMeaningByVocabIdAsync(Guid vocabularyId);
    Task<bool> CheckExistedByVocabularyId(Guid vocabularyId);
}
