namespace Allen.Application;

[RegisterService(typeof(IDeckService))]
public class DeckService(
    IDeckRepository _deckRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IDeckService
{
    public async Task<bool> CheckDeckOwner(Guid deckId, Guid userId)
    {
        return await _deckRepository.CheckDeckOwnerAsync(deckId, userId);
    }

    public async Task<OperationResult> CreateAsync(CreateDeckModel model, Guid userId)
    {
        var entity = _mapper.Map<DeckEntity>(model);
        entity.UserCreateId = userId;

        entity = await _unitOfWork.Repository<DeckEntity>().AddAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(DeckEntity)), "");

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(DeckEntity)), entity.Id);
    }

    public async Task<OperationResult> DeleteByIdAsync(Guid userId, Guid deckId)
    {
        if(deckId == Guid.Empty || !await _unitOfWork.Repository<DeckEntity>().CheckExistByIdAsync(deckId))
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, deckId, nameof(DeckEntity)));

        if (!await CheckDeckOwner(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

        await _unitOfWork.Repository<DeckEntity>().DeleteByIdAsync(deckId);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(DeckEntity)), "");

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(DeckEntity)), deckId);
    }

    public async Task<OperationResult> UpdateAsync(UpdateDeckModel model, Guid deckId, Guid userId)
    {
        var repo = _unitOfWork.Repository<DeckEntity>();

        var entity = await repo.GetByIdAsync(deckId)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, deckId, nameof(DeckEntity)));

        if (!await _deckRepository.CheckDeckOwnerAsync(deckId, userId))
            throw new ForbiddenException(ErrorMessageBase.Forbidden);

         _mapper.Map(model, entity);

        repo.UpdateAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(DeckEntity)), "");

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(DeckEntity)), deckId);
    }
    public async Task<DeckModel> GetByIdAsync(Guid deckId)
    {
        return await _deckRepository.GetDeckModelById(deckId);
    }

    public async Task<DeckPropsModel> GetDeckPropsAsync(Guid deckId)
    {
        return await _deckRepository.GetDeckPropsAsync(deckId);
    }

    public async Task<List<DeckModel>> GetByUserIdAsync(Guid id)
    {
        return await _deckRepository.GetDeckModelByUserId(id);
    }
}