namespace Allen.Application;

[RegisterService(typeof(ITranscriptService))]
public class TranscriptService(
    ITranscriptRepository _repository,
    IUnitOfWork _unitOfWork
) : ITranscriptService
{
    public async Task<OperationResult> CreateAsync(TranscriptEntity entity)
    {
        await _unitOfWork.Repository<TranscriptEntity>().AddAsync(entity);
        return OperationResult.SuccessResult("Created successfully", entity.Id);
    }

    public async Task<OperationResult> UpdateAsync(TranscriptEntity entity)
    {
        var existing = await _unitOfWork.Repository<TranscriptEntity>().GetByIdAsync(entity.Id);
        if (existing == null)
            throw new NotFoundException($"Not found {nameof(TranscriptEntity)} {entity.Id}");

        _unitOfWork.Repository<TranscriptEntity>().UpdateAsync(entity);
        return OperationResult.SuccessResult("Updated successfully", entity.Id);
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        var existing = await _unitOfWork.Repository<TranscriptEntity>().GetByIdAsync(id);
        if (existing == null)
            throw new NotFoundException($"Not found {nameof(TranscriptEntity)} {id}");

        await _unitOfWork.Repository<TranscriptEntity>().DeleteByIdAsync(id);
        return OperationResult.SuccessResult("Deleted successfully", id);
    }

    public async Task<TranscriptEntity?> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.Repository<TranscriptEntity>().GetByIdAsync(id);
    }

    public async Task<IEnumerable<TranscriptEntity>> GetAllAsync()
    {
        return await _unitOfWork.Repository<TranscriptEntity>().GetAllAsync();
    }

    public async Task<IEnumerable<TranscriptEntity>> GetByMediaIdAsync(Guid mediaId)
    {
        return await _repository.GetByMediaIdAsync(mediaId);
    }
}
