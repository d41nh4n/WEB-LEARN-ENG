namespace Allen.Infrastructure;
[RegisterService(typeof(IVocabularyMeaningRepository))]

public class VocabularyMeaningRepository(
    SqlApplicationDbContext context
) : RepositoryBase<VocabularyMeaningEntity>(context), IVocabularyMeaningRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<IEnumerable<VocabularyMeaningEntity>> GetVocabMeaningByVocabIdAsync(Guid vocabularyId)
    {
        return await _context.VocabularyMeanings
            .AsNoTracking()
            .Where(v => v.VocabularyId == vocabularyId)
            .ToListAsync();
    }
    public async Task<bool> CheckExistedByVocabularyId(Guid vocabularyId)
    {
        return await _context.VocabularyMeanings
            .AnyAsync(v => v.VocabularyId == vocabularyId);
    }
}
