namespace Allen.Application;

[RegisterService(typeof(IWritingService))]
public class WritingService(
	IWritingRepository _repository,
	IUnitOfWork _unitOfWork,
	IMapper _mapper,
	IBlobStorageService _blob
) : IWritingService
{
	#region Learning
	public async Task<WritingLearningModel> GetLearningWritingByIdAsync(Guid id)
	{
		var entity = await _repository.GetLearningWritingByIdAsync(id);
		return _mapper.Map<WritingLearningModel>(entity);
	}

	public async Task<WritingLearningModel> GetLearningWritingByLearningIdAsync(Guid id)
	{
		return await _repository.GetLearningWritingByLearningUnitIdAsync(id);
	}

	public async Task<QueryResult<WritingLearningModel>> GetLearningWritingsAsync(QueryInfo queryInfo)
	{
		return await _repository.GetLearningWritingsAsync(queryInfo);
	}

	public async Task<OperationResult> CreateLearningAsync(CreateLearningWritingModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Writing))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		try
		{
			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				var entity = _mapper.Map<WritingEntity>(model);
				entity.LearningUnit.SkillType = SkillType.Writing;
				entity.LearningUnit.LearningUnitType = LearningUnitType.Academy;
				entity.LearningUnit!.LearningUnitStatusType = LearningUnitStatusType.Draft;

				await _unitOfWork.Repository<WritingEntity>().AddAsync(entity);
				await _unitOfWork.SaveChangesAsync();
			});
			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(WritingEntity)));
		}
		catch (Exception)
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(WritingLearningModel)));
		}
	}

	public async Task<OperationResult> UpdateLearningAsync(Guid id, UpdateLearningWritingModel model)
	{
		var entity = await _unitOfWork.Repository<WritingEntity>().GetByIdAsync(id);
		var existingLearningUnit = await _unitOfWork.Repository<LearningUnitEntity>().GetByIdAsync(entity.LearningUnitId);

		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Writing))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		try
		{
			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				_mapper.Map(model, entity);
				_mapper.Map(model.LearningUnit, existingLearningUnit);

				_unitOfWork.Repository<WritingEntity>().UpdateAsync(entity);

				if (!await _unitOfWork.SaveChangesAsync())
				{
					throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(WritingEntity)));
				}
			});

			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(WritingEntity)));
		}
		catch (Exception ex)
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(WritingEntity), ex.InnerException?.Message ?? ex.Message));
		}
	}
	#endregion

	#region Ielts
	public async Task<WritingIeltsModel> GetIeltsWritingByIdAsync(Guid id)
	{
		var entity = await _repository.GetIeltsWritingByIdAsync(id);
		return _mapper.Map<WritingIeltsModel>(entity);
	}

	public async Task<WritingIeltsModel> GetIeltsWritingByLearningIdAsync(Guid id)
	{
		return await _repository.GetIeltsWritingByLearningUnitIdAsync(id);
	}

	public async Task<QueryResult<WritingIeltsModel>> GetIeltsWritingsAsync(QueryInfo queryInfo)
	{
		return await _repository.GetIeltsWritingsAsync(queryInfo);
	}

	public async Task<OperationResult> CreateIeltsAsync(CreateIeltsWritingModel model)
	{
		string? imageUrl = null;
		try
		{
			if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Writing))
				return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

			if (model.SourceUrl != null)
				imageUrl = await _blob.UploadFileAsync(AppConstants.BlobContainerWriting, model.SourceUrl);

			var entity = _mapper.Map<WritingEntity>(model);
			entity.SourceUrl = imageUrl;
			entity.LearningUnit.SkillType = SkillType.Writing;
			entity.LearningUnit.LearningUnitType = LearningUnitType.Ielts;
			entity.LearningUnit!.LearningUnitStatusType = LearningUnitStatusType.Draft;

			await _unitOfWork.Repository<WritingEntity>().AddAsync(entity);
			if (!await _unitOfWork.SaveChangesAsync())
			{
				if (!string.IsNullOrEmpty(imageUrl))
					await _blob.DeleteFileByUrlAsync(imageUrl);

				return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(WritingEntity)));
			}

			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(WritingEntity)));
		}
		catch (Exception ex)
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(WritingIeltsModel)) + $" → {ex.Message}");
		}
	}

	public async Task<OperationResult> UpdateIeltsAsync(Guid id, UpdateIeltsWritingModel model)
	{
		try
		{
			var entity = await _unitOfWork.Repository<WritingEntity>().GetByIdAsync(id);
			var existingLearningUnit = await _unitOfWork.Repository<LearningUnitEntity>().GetByIdAsync(entity.LearningUnitId);

			if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Writing))
				return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				_mapper.Map(model, entity);
				_mapper.Map(model.LearningUnit, existingLearningUnit);

				if (model.SourceUrl != null)
				{
					if (!string.IsNullOrEmpty(entity.SourceUrl))
						await _blob.DeleteFileByUrlAsync(entity.SourceUrl);

					entity.SourceUrl = await _blob.UploadFileAsync(AppConstants.BlobContainerWriting, model.SourceUrl);
				}

				_unitOfWork.Repository<WritingEntity>().UpdateAsync(entity);

				if (!await _unitOfWork.SaveChangesAsync())
					throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(WritingEntity)));
			});

			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(WritingEntity)));
		}
		catch (Exception ex)
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(WritingEntity), ex.InnerException?.Message ?? ex.Message));
		}
	}
	#endregion

	public async Task<OperationResult> DeleteAsync(Guid id)
	{
		var entity = await _unitOfWork.Repository<WritingEntity>().GetByIdAsync(id);
		if (entity == null)
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(WritingEntity), id));

		var imageUrl = entity.SourceUrl;

		await _unitOfWork.Repository<WritingEntity>().DeleteByIdAsync(id);

		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(WritingEntity)));

		if (!string.IsNullOrEmpty(imageUrl))
		{
			await _blob.DeleteFileByUrlAsync(imageUrl);
		}

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(WritingEntity)));
	}
}
