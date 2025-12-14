namespace Allen.Application;

[RegisterService(typeof(IFeedbacksService))]
internal class FeedbacksService(IFeedbacksRepository _repository,
	IBlobStorageService _blobStorageService,
	IUnitOfWork _unitOfWork,
	IMapper _mapper) : IFeedbacksService
{
	public async Task<QueryResult<FeedbackModel>> GetFeedbacksAsync(FeedbackQuery query, QueryInfo queryInfo)
		=> await _repository.GetFeedbacksAsync(query,queryInfo);
	public async Task<QueryResult<FeedbackModel>> GetFeedbacksOfUserAsync(Guid userId, FeedbackQuery query, QueryInfo queryInfo)
		=> await _repository.GetFeedbacksOfUserAsync(userId, query, queryInfo);

	public async Task<FeedbackModel> GetByIdAsync(Guid id)
		=> await _repository.GetFeedbackByIdAsync(id);
	public async Task<FeedbackModel> GetByIdOfUserAsync(Guid userId, Guid id)
		=> await _repository.GetByIdOfUserAsync(userId, id);

	public async Task<OperationResult> CreateAsync(Guid userId, CreateOrUpdateFeedbackModel model)
	{
		var now = DateTime.UtcNow.AddHours(7); 
		var startOfDay = now.Date;             
		var endOfDay = startOfDay.AddDays(1);  

		var count = await _unitOfWork.Repository<FeedbackEntity>()
			.GetCountAsync(x => x.UserId == userId
				&& x.CreatedAt >= startOfDay
				&& x.CreatedAt < endOfDay);
		if (count > 3)
		{
			return OperationResult.Failure("Bạn đã vượt quá số lần feedback trong ngày");
		}

		var categoryEntity = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(model.CategoryId);
		if (categoryEntity != null)
		{
			if (categoryEntity.SkillType != SkillType.Feedback)
			{
				return OperationResult.Failure("Category không thuộc loại Feedback");
			}
		}

		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.CategoryId))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.Title));

		var entity = _mapper.Map<FeedbackEntity>(model);
		entity.UserId = userId;
		if (model.Image != null)
		{
			var image =  await _blobStorageService.UploadFileAsync(AppConstants.BlobContainer, model.Image);
			entity.Image = image;
		}

		await _unitOfWork.Repository<FeedbackEntity>().AddAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(FeedbackEntity)));

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(FeedbackEntity)));
	}
	public async Task<OperationResult> UpdateAsync(Guid id, Guid userId, CreateOrUpdateFeedbackModel model)
	{
		var categoryEntity = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(model.CategoryId);
		if (categoryEntity != null)
		{
			if (categoryEntity.SkillType != SkillType.Feedback)
			{
				return OperationResult.Failure("Category không thuộc loại Feedback");
			}
		}
		var entity = await _unitOfWork.Repository<FeedbackEntity>().GetByIdAsync(id);
		entity.UserId = userId;
		_mapper.Map(model, entity);

		if (model.Image != null)
		{
			if(entity.Image != null) 
			{
				await _blobStorageService.DeleteFileByUrlAsync(entity.Image);
			}

			var image = await _blobStorageService.UploadFileAsync(AppConstants.BlobContainer, model.Image);
			entity.Image = image;
		}
		_unitOfWork.Repository<FeedbackEntity>().UpdateAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(FeedbackEntity)));

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(FeedbackEntity)));
	}
	public async Task<OperationResult> DeleteAsync(Guid id)
	{
		var entity = await _unitOfWork.Repository<FeedbackEntity>().GetByIdAsync(id);
		if (entity.Image != null)
		{
			await _blobStorageService.DeleteFileByUrlAsync(entity.Image);
		}

		await _unitOfWork.Repository<FeedbackEntity>().DeleteByIdAsync(id);
		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(FeedbackEntity)));
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(FeedbackEntity)));
	}
}
