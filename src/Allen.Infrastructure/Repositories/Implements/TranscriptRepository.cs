namespace Allen.Infrastructure;

[RegisterService(typeof(ITranscriptRepository))]
public class TranscriptRepository(SqlApplicationDbContext context)
    : RepositoryBase<TranscriptEntity>(context), ITranscriptRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<IEnumerable<TranscriptEntity>> GetByMediaIdAsync(Guid mediaId)
    {
        return await _context.Transcripts
            .Include(x => x.Media)
            .Where(x => x.MediaId == mediaId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync();
    }
}
