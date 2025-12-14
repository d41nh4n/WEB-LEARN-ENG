namespace Allen.Infrastructure;

[RegisterService(typeof(IVocabularyTagRepository))]
public class VocabularyTagRepository(SqlApplicationDbContext context) : RepositoryBase<VocabularyTagEntity>(context), IVocabularyTagRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<IEnumerable<VocabularyTagEntity>> GetVocabularyTagByVocabIdAsync(Guid vocabularyId)
    {
        return await _context.VocabularyTags
            .AsNoTracking()
            .Where(v => v.VocabularyId == vocabularyId)
            .ToListAsync();
    }
    public async Task<bool> CheckExistedByVocabularyId(Guid vocabularyId)
    {
        return await _context.VocabularyTags
            .AnyAsync(v => v.VocabularyId == vocabularyId);
    }

    public async Task<IEnumerable<Guid>> GetTagsIdByVocabularyIdAsync(Guid vocabularyId)
    {
        return await _context.VocabularyTags
            .AsNoTracking()
            .Where(x => x.VocabularyId == vocabularyId)
            .Select(x => x.Id)
            .ToListAsync();
    }
}
