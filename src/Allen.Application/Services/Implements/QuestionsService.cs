namespace Allen.Application;

[RegisterService(typeof(IQuestionsService))]
public class QuestionsService(
    IQuestionsRepository _repository,
    IBlobStorageService _blobStorageService,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IQuestionsService
{
    public async Task<QueryResult<QuestionModel>> GetQuestionsAsync(Guid moduleItemId, QueryInfo queryInfo)
    {
        return await _repository.GetQuestionsAsync(moduleItemId, queryInfo);
    }
    public async Task<QuestionModel> GetQuestionByIdAsync(Guid id)
    {
        return await _repository.GetQuestionByIdAsync(id);
    }
	public async Task<List<AnswerResult>> GetAnswersOfUserByLearningIdAsync(Guid learningUnitId, Guid userId)
	{
		var questions = await _repository.GetQuestionsByLearningUnitIdAsync(learningUnitId, true);
		var subQuestions = await _repository.GetSubQuestionsByLearningUnitIdAsync(learningUnitId, true);

		var userAnswers = await _repository.GetAnswersOfUserByLearningIdAsync(learningUnitId, userId);

		var resultItems = new List<AnswerResult>();

		foreach (var sub in subQuestions)
		{
			var ans = userAnswers.FirstOrDefault(a => a.SubQuestionId == sub.Id);

			resultItems.Add(new AnswerResult
			{
				QuestionId = sub.QuestionId,
				SubQuestionId = sub.Id,
				UserAnswer = ans?.UserAnswer ?? "",       
				CorrectAnswer = sub.CorrectAnswer ?? "",
				IsCorrect = ans?.IsCorrect ?? false,
                StartTextIndex = sub.StartTextIndex,
                EndTextIndex = sub.EndTextIndex,
			});
		}

		var questionWithoutSub = questions.Where(q => !subQuestions.Any(s => s.QuestionId == q.Id));

		foreach (var q in questionWithoutSub)
		{
			var ans = userAnswers.FirstOrDefault(a => a.QuestionId == q.Id);

			resultItems.Add(new AnswerResult
			{
				QuestionId = q.Id,
				SubQuestionId = null,
				UserAnswer = ans?.UserAnswer ?? "",
				CorrectAnswer = q.CorrectAnswer ?? "",
				IsCorrect = ans?.IsCorrect ?? false,
                StartTextIndex = q.StartTextIndex,
                EndTextIndex = q.EndTextIndex,
			});
		}

		return resultItems
			.OrderBy(r => r.QuestionId)
			.ThenBy(r => r.SubQuestionId)
			.ToList();
	}

	public async Task<OperationResult> CreateQuestionAsync(CreateOrUpdateQuestionModel model)
    {
        var contentUrl = string.Empty;
		if (model.File != null)
        {
			contentUrl = await _blobStorageService.UploadFileAsync(AppConstants.BlobContainerFileMp3, model.File!);
		}
        var entity = _mapper.Map<QuestionEntity>(model);
        entity.ContentUrl = contentUrl;
        await _unitOfWork.Repository<QuestionEntity>().AddAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            await _blobStorageService.DeleteFileByUrlAsync(contentUrl);
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(QuestionEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(QuestionEntity)));
    }
    public async Task<OperationResult> CreateQuestionForReadingAsync(CreateOrUpdateQuestionForReadingModel model)
    {
        //if (!await _unitOfWork.Repository<LearningUnitEntity>().CheckExistByIdAsync(model.ModuleItemId))
        //    return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(LearningUnitEntity), model.ModuleItemId));

        var entity = _mapper.Map<QuestionEntity>(model);
        entity.Id = Guid.NewGuid();
        entity.ModuleType = LearningModuleType.ReadingPassage;
        if (entity.SubQuestions != null)
        {
            foreach (var item in entity.SubQuestions)
            {
                item.Id = Guid.NewGuid();
                item.QuestionId = entity.Id;
            }
        }
        await _unitOfWork.Repository<QuestionEntity>().AddAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(QuestionEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(QuestionEntity)));
    }

    public async Task<OperationResult> CreateQuestionForListeningAsync(CreateOrUpdateQuestionForListeningModel model)
    {
        //if (!await _unitOfWork.Repository<LearningUnitEntity>().CheckExistByIdAsync(model.ModuleItemId))
        //    return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(LearningUnitEntity), model.ModuleItemId));

        var entity = _mapper.Map<QuestionEntity>(model);
        entity.Id = Guid.NewGuid();
        entity.ModuleType = LearningModuleType.ListeningLesson;
        if (entity.SubQuestions != null)
        {
            foreach (var item in entity.SubQuestions)
            {
                item.Id = Guid.NewGuid();
                item.QuestionId = entity.Id;
            }
        }
        await _unitOfWork.Repository<QuestionEntity>().AddAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(QuestionEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(QuestionEntity)));
    }
	public async Task<OperationResult> CreateQuestionForSpeakingAsync(CreateOrUpdateQuestionForSpeakingModel model)
	{
		var entity = _mapper.Map<QuestionEntity>(model);
		entity.ModuleType = LearningModuleType.SpeakingPractice;
		await _unitOfWork.Repository<QuestionEntity>().AddAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(QuestionEntity)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(QuestionEntity)));
	}
	public async Task<OperationResult> CreateQuestionsAsync(List<CreateOrUpdateQuestionModel> models)
    {
        var entities = _mapper.Map<List<QuestionEntity>>(models);
        await _unitOfWork.Repository<QuestionEntity>().AddRangeAsync(entities);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(QuestionEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(QuestionEntity)));
    }

    public async Task<OperationResult> UpdateQuestionAsync(Guid id, CreateOrUpdateQuestionModel model)
    {
        var entity = await _unitOfWork.Repository<QuestionEntity>().GetByIdAsync(id);

        string? oldFile = null;
        string? newFile = null;
        if (!string.IsNullOrWhiteSpace(entity.ContentUrl) &&
            (model.QuestionType == QuestionType.Listening.ToString() || model.QuestionType == QuestionType.Speaking.ToString()))
        {
            oldFile = entity.ContentUrl;
            newFile = await _blobStorageService.UploadFileAsync(AppConstants.BlobContainerFileMp3, model.File!);
        }

        _mapper.Map(model, entity);

        entity.ContentUrl = newFile;

        _unitOfWork.Repository<QuestionEntity>().UpdateAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
        {
            if (!string.IsNullOrWhiteSpace(newFile))
                await _blobStorageService.DeleteFileByUrlAsync(newFile);

            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(QuestionEntity)));
        }

        if (!string.IsNullOrWhiteSpace(oldFile))
            await _blobStorageService.DeleteFileByUrlAsync(oldFile);

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(QuestionEntity)));
    }
    public async Task<OperationResult> UpdateQuestionForReadingAsync(Guid id, CreateOrUpdateQuestionForReadingModel model)
    {
        var repo = _unitOfWork.Repository<QuestionEntity>();
        var entity = await repo.GetByIdAsync(id, [q => q.SubQuestions]);

        if (entity == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(QuestionEntity), id));

        _mapper.Map(model, entity);

        repo.UpdateAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(QuestionEntity)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(QuestionEntity)));
    }

    public async Task<OperationResult> UpdateQuestionForListeningAsync(Guid id, CreateOrUpdateQuestionForListeningModel model)
    {
        var repo = _unitOfWork.Repository<QuestionEntity>();
        var entity = await repo.GetByIdAsync(id, [q => q.SubQuestions]);

        if (entity == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(QuestionEntity), id));

        _mapper.Map(model, entity);

        repo.UpdateAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(QuestionEntity)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(QuestionEntity)));
    }
    public async Task<OperationResult> DeleteQuestionAsync(Guid id)
    {
        await _unitOfWork.Repository<QuestionEntity>().DeleteByIdAsync(id);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(QuestionEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(QuestionEntity)));
    }
}
