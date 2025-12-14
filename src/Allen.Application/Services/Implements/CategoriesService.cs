namespace Allen.Application;

[RegisterService(typeof(ICategoriesService))]
public class CategoriesService
    (ICategoriesRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : ICategoriesService
{
    public async Task<QueryResult<CategoryModel>> GetCategoriesAsync(CategoryQuery categoryQuery)
    {
        return await _repository.GetCategoriesAsync(categoryQuery);
    }

    public async Task<CategoryModel> GetByIdAsync(Guid id)
    {
        return await _repository.GetCategoryByIdAsync(id);
    }

    public async Task<OperationResult> CreateAsync(CreateOrUpdateCategoryModel model)
    {

        if(await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Name == model.Name && x.SkillType == Enum.Parse<SkillType>(model.SkillType)))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(CategoryEntity), model.Name));

		var entity = _mapper.Map<CategoryEntity>(model);
		await _unitOfWork.Repository<CategoryEntity>().AddAsync(entity);
		if(!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(CategoryEntity)));

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(CategoryEntity)));
    }

    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateCategoryModel model)
    {
        if (await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Name == model.Name && x.SkillType == Enum.Parse<SkillType>(model.SkillType)))
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(CategoryEntity), model.Name));

        var entity = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(id);

        _mapper.Map(model, entity);
        _unitOfWork.Repository<CategoryEntity>().UpdateAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(CategoryEntity)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(CategoryEntity)));
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        await _unitOfWork.Repository<CategoryEntity>().DeleteByIdAsync(id);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(CategoryEntity)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(CategoryEntity)));
    }
}
