
namespace Allen.Application;

[RegisterService(typeof(IPackageService))]
public class PackageService(
    IPackageRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper
) : IPackageService
{
    public async Task<QueryResult<PackageModel>> GetPackagesAsync(QueryInfo queryInfo, PackageQuery query)
    {
        return await _repository.GetPackagesAsync(queryInfo, query);
    }

    public async Task<PackageModel> GetPackageByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        var result = _mapper.Map<PackageModel>(entity);
        return result;
    }

    public async Task<OperationResult> CreateAsync(CreatePackageModel model)
    {
        // Không được trùng tên
        var sameName = await _repository.FindAsync(p => p.Name == model.Name);
        if (sameName != null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(PackageModel), model.Name ?? ""));

        // Không được trùng giá & điểm (dù khác tên)
        var samePricePoint = await _repository.FindAsync(p => p.Price == model.Price && p.Points == model.Points);
        if (samePricePoint != null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists,
                nameof(PackageModel), $"(Price: {model.Price}, Points: {model.Points})"));

        var entity = _mapper.Map<PackageEntity>(model);
        model.IsActive = true;

        await _repository.AddAsync(entity);

        if(!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(PackageModel)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(PackageModel)), new { entity.Id });
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpdatePackageModel model)
    {
        var entity = await _repository.GetByIdAsync(id);
        if(entity == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(PackageModel), id));

        // Không được trùng tên (ngoại trừ bản thân)
        var sameName = await _repository.FindAsync(p => p.Id != id && p.Name == model.Name);
        if (sameName != null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(PackageModel), model.Name ?? ""));

        // Không được trùng giá & điểm (ngoại trừ bản thân)
        var samePricePoint = await _repository.FindAsync(p => p.Id != id && p.Price == model.Price && p.Points == model.Points);
        if (samePricePoint != null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists,
                nameof(PackageModel), $"(Price: {model.Price}, Points: {model.Points})"));

        _mapper.Map(model, entity);
        _repository.UpdateAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(PackageModel), id));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(PackageModel), id));
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(PackageModel), id));

        await _repository.DeleteByIdAsync(id);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(PackageModel), id));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(PackageModel), id));
    }
}
