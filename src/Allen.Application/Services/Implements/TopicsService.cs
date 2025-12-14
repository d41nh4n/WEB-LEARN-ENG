namespace Allen.Application;

[RegisterService(typeof(ITopicService))]
public class TopicsService(
    ITopicRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : ITopicService
{
    // =========================
    // 1. READ
    // =========================
    public async Task<QueryResult<TopicModel?>> GetTopicsAsync(QueryInfo queryInfo)
    {
        return await _repository.GetTopicsAsync(queryInfo);
    }

    public async Task<TopicModel?> GetByIdAsync(Guid id)
    {
        return await _repository.GetTopicByIdAsync(id)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TopicEntity)));
    }

    // =========================
    // 2. CREATE
    // =========================
    public async Task<OperationResult> CreateAsync(CreateTopicModel model)
    {
        var topicNameNormalize = StringExtensions.ConvertToCase(model.TopicName!, StringCaseType.Lower);

        if (await _unitOfWork.Repository<TopicEntity>().CheckExistAsync(x => x.TopicName == topicNameNormalize))
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TopicEntity)));

        var entity = _mapper.Map<TopicEntity>(model);

        await _unitOfWork.Repository<TopicEntity>().AddAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(TopicEntity));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(TopicEntity)), entity);
    }


    // =========================
    // 3. UPDATE
    // =========================
    public async Task<OperationResult> UpdateAsync(Guid id, UpdateTopicModel model)
    {
        var topicExisted = await _unitOfWork.Repository<TopicEntity>().GetByIdAsync(id);
        var nameTagNormalize = StringExtensions.ConvertToCase(model.TopicName!, StringCaseType.Lower);

        _mapper.Map(model, topicExisted);

        _unitOfWork.Repository<TopicEntity>().UpdateAsync(topicExisted);

        if (!await _unitOfWork.SaveChangesAsync())
            throw new InternalServerException(ErrorMessageBase.UpdateFailure, nameof(TopicEntity));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(TopicEntity)));
    }

    // =========================
    // 4. DELETE
    // =========================
    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        if (!await _unitOfWork.Repository<TopicEntity>().CheckExistByIdAsync(id))
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TopicEntity)));
        }

        await _unitOfWork.Repository<TopicEntity>().DeleteByIdAsync(id);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.DeleteFailure, nameof(TopicEntity));

        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(TopicEntity)), id);
    }
}
