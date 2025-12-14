namespace Allen.Infrastructure;

public interface IWritingSubmissionRepository : IRepositoryBase<WritingSubmissionEntity>
{
    Task<List<WritingSubmissionEntity>> GetWritingSubmissionByUserAsync(Guid userId, Guid writingId);
}