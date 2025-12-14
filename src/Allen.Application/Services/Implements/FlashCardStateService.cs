
namespace Allen.Application;

[RegisterService(typeof(IFlashCardsStateService))]
public class FlashCardStateService
    (IFlashCardStateRepository _flashCardStateRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IFlashCardsStateService
{
    public async Task<FlashCardStateEntity> CreateAsync(FlashCardStateModel model)
    {
        var entity = _mapper.Map<FlashCardStateEntity>(model);

        await _unitOfWork.Repository<FlashCardStateEntity>().AddAsync(entity);

        return entity;
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        await _flashCardStateRepository.DeleteByFlashCardIdAsync(id);

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(FlashCardStateEntity)), id);
    }

    public Task<QueryResult<FlashCardStateModel>> GetByFlashCardIdAsync(Guid flashCardId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<FlashCardStateEntity>> GetFlashCardsStateByDeckId(Guid flashCardId) 
        => await _flashCardStateRepository.GetFlashCardsStateByDeckIdAsync(flashCardId) ?? throw new NotFoundException(
            ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(FlashCardStateEntity)));

    public OperationResult ResetStateCards(List<FlashCardStateEntity> flashCardState)
    {
        foreach (var state in flashCardState)
        {
            state.Stability = 0;
            state.Difficulty = 5.0;
            state.Repetition = 0;
            state.Interval = 0;
            state.NextReviewDate = DateTime.UtcNow;
            state.LastReviewedAt = null;
            state.LastRating = null;

            _unitOfWork.Repository<FlashCardStateEntity>().UpdateAsync(state);
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(FlashCardStateEntity)));
    }
}
