namespace Allen.Infrastructure;

public interface ITranscriptRepository : IRepositoryBase<TranscriptEntity>
{
    Task<IEnumerable<TranscriptEntity>> GetByMediaIdAsync(Guid mediaId);
}
