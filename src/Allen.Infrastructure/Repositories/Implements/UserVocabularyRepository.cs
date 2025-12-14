
namespace Allen.Infrastructure;

[RegisterService(typeof(IUserVocabularyRepository))]
public class UserVocabularyRepository(SqlApplicationDbContext context) : RepositoryBase<UserVocabularyEntity>(context), IUserVocabularyRepository
{
    private readonly SqlApplicationDbContext _context = context;
    public async Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserAsync(QueryInfo queryInfo, Guid userId)
    {
        var query = _context.UserVocabularies
            .AsNoTracking()
            .Include(uv => uv.Vocabulary)
                .ThenInclude(v => v.VocabularyMeanings)
            .Where(uv => uv.UserId == userId)
            .OrderByDescending(uv => uv.CreatedAt)
            .Select(uv => new VocabularyOfUserModel
            {
                Id = uv.Vocabulary.Id,
                Word = uv.Vocabulary.Word,
                Level = uv.Vocabulary.Level.ToString(),
                VocabularyMeanings = uv.Vocabulary.VocabularyMeanings
                    .Select(m => new VocabularyMeaningModel
                    {
                        Id = m.Id,
                        PartOfSpeech = m.PartOfSpeech.ToString(),
                        Pronunciation = m.Pronunciation,
                        Audio = m.Audio,
                        DefinitionEN = m.DefinitionEN,
                        DefinitionVN = m.DefinitionVN,
                        Example1 = m.Example1,
                        Example2 = m.Example2,
                        Example3 = m.Example3
                    })
                    .ToList()
            });

        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = queryInfo.NeedTotalCount
            ? await query.CountAsync()
            : 0;

        return new QueryResult<VocabularyOfUserModel>
        {
            Data = entities,
            TotalCount = totalCount
        };
    }
    public async Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserByTopicIdAsync(
    QueryInfo queryInfo, Guid userId, Guid topicId)
    {
        var query = _context.UserVocabularies
            .AsNoTracking()
            .Include(uv => uv.Vocabulary)
                .ThenInclude(v => v.VocabularyMeanings)
            .Where(uv => uv.UserId == userId && uv.Vocabulary.TopicId == topicId)
            .OrderByDescending(uv => uv.CreatedAt)
            .Select(uv => new VocabularyOfUserModel
            {
                Id = uv.Vocabulary.Id,
                Word = uv.Vocabulary.Word,
                Level = uv.Vocabulary.Level.ToString(),
                VocabularyMeanings = uv.Vocabulary.VocabularyMeanings
                    .Select(m => new VocabularyMeaningModel
                    {
                        Id = m.Id,
                        PartOfSpeech = m.PartOfSpeech.ToString(),
                        Pronunciation = m.Pronunciation,
                        Audio = m.Audio,
                        DefinitionEN = m.DefinitionEN,
                        DefinitionVN = m.DefinitionVN,
                        Example1 = m.Example1,
                        Example2 = m.Example2,
                        Example3 = m.Example3
                    })
                    .ToList()
            });

        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = queryInfo.NeedTotalCount
            ? await query.CountAsync()
            : 0;

        return new QueryResult<VocabularyOfUserModel>
        {
            Data = entities,
            TotalCount = totalCount
        };
    }
}
