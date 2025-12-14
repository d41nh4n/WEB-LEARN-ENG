namespace Allen.Application;

[RegisterService(typeof(ILearningUnitsService))]
public class LearningUnitsService(
	ILearningUnitsRepository _repository,
	IUnitOfWork _unitOfWork,
	IMapper _mapper) : ILearningUnitsService
{
	public async Task<QueryResult<LearningUnit>> GetAllWithPagingAsync(QueryInfo queryInfo)
	{
		return await _repository.GetAllWithPagingAsync(queryInfo);
	}
	public async Task<QueryResult<LearningUnit>> GetByCategoryIdAsync(Guid id, QueryInfo queryInfo)
	{
		return await _repository.GetByCategoryIdAsync(id, queryInfo);
	}
	public async Task<QueryResult<LearningUnit>> GetByFiltersAsync(LearningUnitQuery learningUnitQuery, QueryInfo queryInfo)
	{
		return await _repository.GetByFiltersAsync(learningUnitQuery, queryInfo);
	}

	public async Task<LearningUnit> GetByIdAsync(Guid id)
	{
		return await _repository.GetLearningUnitByIdAsync(id);
	}

	public async Task<OperationResult> CreateAsync(CreateOrUpdateLearningUnitModel model)
	{
		var category = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(model.CategoryId);
		var entity = _mapper.Map<LearningUnitEntity>(model);
		entity.LearningUnitStatusType = LearningUnitStatusType.Draft;
		if (category.SkillType != entity.SkillType)
		{
			return OperationResult.Failure(ErrorMessageBase.Format("The SkillType of LearningUnit must match the SkillType of its Category."));
		}
		await _unitOfWork.Repository<LearningUnitEntity>().AddAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(LearningUnit)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(LearningUnit)), entity.Id);
	}
	public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateLearningUnitModel model)
	{
		var category = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(model.CategoryId);
		var learningUnit = await _unitOfWork.Repository<LearningUnitEntity>().GetByIdAsync(id);
		var entity = _mapper.Map(model, learningUnit);
		if (category.SkillType != entity.SkillType)
		{
			return OperationResult.Failure(ErrorMessageBase.Format("The SkillType of LearningUnit must match the SkillType of its Category."));
		}
		_unitOfWork.Repository<LearningUnitEntity>().UpdateAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(LearningUnit)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(LearningUnit)));
	}
	public async Task<OperationResult> DeleteAsync(Guid id)
	{
		await _unitOfWork.Repository<LearningUnitEntity>().DeleteByIdAsync(id);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(LearningUnit)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(LearningUnit)));
	}

	public async Task<OperationResult> UpdateUnitStatusAsync(Guid id, UpdateLearningUnitStatusModel model)
	{
		var learningUnit = await _unitOfWork.Repository<LearningUnitEntity>().GetByIdAsync(id);
		var entity = _mapper.Map(model, learningUnit);
		_unitOfWork.Repository<LearningUnitEntity>().UpdateAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(LearningUnit)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(LearningUnit)));
	}
}
