namespace Allen.Infrastructure;

[RegisterService(typeof(IWritingSubmissionRepository))]
public class WritingSubmissionRepository(
    SqlApplicationDbContext context
) : RepositoryBase<WritingSubmissionEntity>(context), IWritingSubmissionRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<List<WritingSubmissionEntity>> GetWritingSubmissionByUserAsync(Guid userId, Guid writingId)
    {
        return await _context.WritingSubmissions
            .Where(ws => ws.UserId == userId && ws.WritingId == writingId)
            .ToListAsync();
    }
}