namespace Allen.Application;

[RegisterService(typeof(ITagService))]
public class TagService(
    ITagRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : ITagService
{
    // =========================
    // 1. CREATE
    // =========================
    public async Task<OperationResult> CreateAsync(CreateTagModel model)
    {
        var nameTagNormalize = StringExtensions.ConvertToCase(model.NameTag!, StringCaseType.Lower);
        if (await _unitOfWork.Repository<TagEntity>().CheckExistAsync(x => x.NameTag == nameTagNormalize))
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(TagEntity)));
        }

        var entity = _mapper.Map<TagEntity>(model);

        entity = await _unitOfWork.Repository<TagEntity>().AddAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(TagEntity));
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(TagEntity)), entity);
    }

    // =========================
    // 2. UPDATE
    // =========================
    public async Task<OperationResult> UpdateAsync(UpdateTagModel model, Guid id)
    {
        var tagExisted = await _unitOfWork.Repository<TagEntity>().GetByIdAsync(id);
        var nameTagNormalize = StringExtensions.ConvertToCase(model.NameTag!, StringCaseType.Lower);

        if (tagExisted == null)
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TagEntity), id));
        }

        if (string.Equals(nameTagNormalize, tagExisted.NameTag))
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(TagEntity), nameTagNormalize));
        }
        _mapper.Map(model, tagExisted);
        tagExisted.NameTag = nameTagNormalize;

        _unitOfWork.Repository<TagEntity>().UpdateAsync(tagExisted);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.UpdateFailure, nameof(TagEntity));
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(TagEntity)), tagExisted);
    }

    // =========================
    // 3. DELETE
    // =========================
    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        if (!await _unitOfWork.Repository<TagEntity>().CheckExistByIdAsync(id))
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TagEntity), id));
        }

        await _unitOfWork.Repository<TagEntity>().DeleteByIdAsync(id);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new InternalServerException(ErrorMessageBase.DeleteFailure, nameof(TagEntity));
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(TagEntity)), id);
    }

    // =========================
    // 4. READ
    // =========================
    public async Task<TagModel?> GetByIdAsync(Guid tagId)
    {
        var entity = await _unitOfWork.Repository<TagEntity>().GetByIdAsync(tagId);
        return _mapper.Map<TagModel>(entity)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TagEntity)));
    }

    // =========================
    // 6. GET ALL
    // =========================
    public async Task<QueryResult<TagModel?>> GetTagsAsync(QueryInfo queryInfo)
    {
        return await _repository.GetTagsAsync(queryInfo);
    }

    // =========================
    // 7. counting how many tags exist in DB given a list of IDs.
    // =========================
    public async Task<bool> CheckAllTagsExistAsync(List<Guid> tagsId)
    {
        if (tagsId == null || tagsId.Count == 0)
            return false;

        var count = await _repository
            .CountExistingTagsAsync(tagsId);

        return count == tagsId.Count;
    }

    public async Task<List<Guid>> CheckNotExistedTagAsync(List<Guid> tagsId)
    {
        if (tagsId == null || tagsId.Count == 0)
            return [];

        // Lấy ra các TagId tồn tại trong DB
        var existedIds = await _repository.GetAllTagsAsync(tagsId);

        // Trả về các TagId KHÔNG tồn tại
        return tagsId.Except(existedIds).ToList();
    }
}
