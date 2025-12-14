namespace Allen.Application;
public interface ITranscriptService
{
    Task<OperationResult> CreateAsync(TranscriptEntity entity);
    Task<OperationResult> UpdateAsync(TranscriptEntity entity);
    Task<OperationResult> DeleteAsync(Guid id);
    Task<TranscriptEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TranscriptEntity>> GetAllAsync();
    Task<IEnumerable<TranscriptEntity>> GetByMediaIdAsync(Guid mediaId);
}
