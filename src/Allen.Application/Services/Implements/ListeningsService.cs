namespace Allen.Application;

[RegisterService(typeof(IListeningsService))]
public class ListeningsService(
	IListeningsRepository _repository,
	IQuestionsRepository _questionRepository,
	IUnitOfWork _unitOfWork,
	IBlobStorageService _blobStorageService,
	IMapper _mapper
	) : IListeningsService
{
	public async Task<ListeningModel> GetByLearningUnitIdAsync(Guid learningUnitId)
		=> await _repository.GetByLearningUnitId(learningUnitId);
	public async Task<ListeningForIeltsModel> GetByLearningUnitIdForIeltsAsync(Guid learningUnitId, GetByLearningUnitIdForIeltsQuery query)
		=> await _repository.GetByLearningUnitIdForIeltsAsync(learningUnitId, query);
	public async Task<OperationResult> CreateLearningAsync(CreateListeningForLearningModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Listening))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		var learningUnitEntity = _mapper.Map<LearningUnitEntity>(model.LearningUnit);
		var listeningEntity = _mapper.Map<ListeningEntity>(model);
		listeningEntity.LearningUnit!.SkillType = SkillType.Listening;
		listeningEntity.LearningUnit!.LearningUnitType = LearningUnitType.Academy;
		listeningEntity.LearningUnit!.LearningUnitStatusType = LearningUnitStatusType.Draft;

		try
		{
			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				await _unitOfWork.Repository<ListeningEntity>().AddAsync(listeningEntity);
				await _unitOfWork.SaveChangesAsync();
			});
			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ListeningEntity)));
		}
		catch
		{
			await _blobStorageService.DeleteFileByUrlAsync(listeningEntity.Media!.SourceUrl);
			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ListeningEntity)));
		}
	}

	public async Task<OperationResult> CreateListeningForIeltsAsync(CreateListeningForIeltsModel model)
	{
		if (!await _unitOfWork.Repository<LearningUnitEntity>().CheckExistAsync(x => x.Id == model.LearningUnitId && x.SkillType == SkillType.Listening))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(LearningUnitEntity), model.LearningUnitId));

		var existingSections = await _unitOfWork.Repository<ListeningEntity>()
			.GetListByConditionAsync(x => x.LearningUnitId == model.LearningUnitId);

		if (!existingSections.Any() && model.SectionIndex != 1)
		{
			return OperationResult.Failure("The first Listening section must have SectionIndex = 1.");
		}
		if (existingSections.Any(x => x.SectionIndex == model.SectionIndex))
		{
			return OperationResult.Failure($"SectionIndex {model.SectionIndex} already exists.");
		}

		//var maxIndex = existingSections.Any() ? existingSections.Max(x => x.SectionIndex) : 0;

		//if (model.SectionIndex != maxIndex + 1)
		//{
		//	return OperationResult.Failure($"Invalid SectionIndex. The next valid index should be {maxIndex + 1}.");
		//}

		var entity = _mapper.Map<ListeningEntity>(model);
		await _unitOfWork.Repository<ListeningEntity>().AddAsync(entity);

		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ListeningEntity)));

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ListeningEntity)), entity.Id);
	}

	public async Task<OperationResult> CreateListeningsForIeltsAsync(CreateListeningsForIeltsModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Listening))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		var learningUnitEntity = _mapper.Map<LearningUnitEntity>(model.LearningUnit);
		learningUnitEntity.SkillType = SkillType.Listening;
		learningUnitEntity.LearningUnitType = LearningUnitType.Ielts;

		await _unitOfWork.Repository<LearningUnitEntity>().AddAsync(learningUnitEntity);
		//await _unitOfWork.SaveChangesAsync();

		var listeningEntities = _mapper.Map<List<ListeningEntity>>(model.Data);

		foreach (var entity in listeningEntities)
		{
			entity.LearningUnitId = learningUnitEntity.Id;
		}

		await _unitOfWork.Repository<ListeningEntity>().AddRangeAsync(listeningEntities);

		var allQuestionEntities = new List<QuestionEntity>();

		for (int i = 0; i < model.Data.Count; i++)
		{
			var sectionModel = model.Data[i];
			var sectionEntity = listeningEntities[i];

			if (sectionModel.Questions == null || sectionModel.Questions.Count == 0)
				continue;

			foreach (var q in sectionModel.Questions)
			{
				q.Id = Guid.NewGuid();
				q.ModuleItemId = sectionEntity.Id;
				q.ModuleType = LearningModuleType.ListeningLesson;

				if (q.SubQuestions != null && q.SubQuestions.Any())
				{
					foreach (var sub in q.SubQuestions)
					{
						sub.QuestionId = q.Id;
					}
				}
			}

			var questionEntities = _mapper.Map<List<QuestionEntity>>(sectionModel.Questions);
			allQuestionEntities.AddRange(questionEntities);
		}

		if (allQuestionEntities.Any())
			await _unitOfWork.Repository<QuestionEntity>().AddRangeAsync(allQuestionEntities);

		var success = await _unitOfWork.SaveChangesAsync();

		if (!success)
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ListeningEntity)));

		return OperationResult.SuccessResult(
			ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, $"{model.Data.Count} Listening Sections")
		);
	}

	public async Task<OperationResult> SubmitIeltsAsync(SubmitIeltsModel model)
	{
		var questions = await _questionRepository.GetQuestionsByLearningUnitIdAsync(model.LearningUnitId, false);
		var subQuestions = await _questionRepository.GetSubQuestionsByLearningUnitIdAsync(model.LearningUnitId, false);

		var resultItems = new List<AnswerResult>();
		int correctCount = 0;

		foreach (var ans in model.Answers)
		{
			string correctAnswer = "";

			if (ans.SubQuestionId.HasValue)
			{
				var sub = subQuestions.FirstOrDefault(s => s.Id == ans.SubQuestionId);
				if (sub == null) continue;
				correctAnswer = sub.CorrectAnswer ?? "";
			}
			else
			{
				var q = questions.FirstOrDefault(q => q.Id == ans.QuestionId);
				if (q == null) continue;
				correctAnswer = q.CorrectAnswer ?? "";
			}

			bool isCorrect = string.Equals(correctAnswer.Trim(), ans.Answer?.Trim(), StringComparison.OrdinalIgnoreCase);
			if (isCorrect) correctCount++;

			resultItems.Add(new AnswerResult
			{
				QuestionId = ans.QuestionId,
				SubQuestionId = ans.SubQuestionId,
				UserAnswer = ans.Answer,
				CorrectAnswer = correctAnswer,
				IsCorrect = isCorrect
			});
		}

		int totalQuestions = questions.Count(q => q.SubQuestions == null || q.SubQuestions.Count == 0)
							+ subQuestions.Count;

		double rawScore = (double)correctCount / totalQuestions * 40;
		double band = ConvertToBand(rawScore);

		var oldAttempts = await _unitOfWork.Repository<UserTestAttemptEntity>()
			.GetListByConditionAsync(x => x.UserId == model.UserId && x.LearningUnitId == model.LearningUnitId);

		if (oldAttempts != null && oldAttempts.Any())
		{
			foreach (var attempt in oldAttempts)
			{
				var oldAnswers = await _unitOfWork.Repository<UserAnswerEntity>()
					.GetListByConditionAsync(a => a.AttemptId == attempt.Id);

				if (oldAnswers.Any())
					_unitOfWork.Repository<UserAnswerEntity>().DeleteRangeAsync(oldAnswers);

				await _unitOfWork.Repository<UserTestAttemptEntity>().DeleteByIdAsync(attempt.Id);
			}
		}

		var attemptId = Guid.NewGuid();
		var newAttempt = new UserTestAttemptEntity
		{
			Id = attemptId,
			UserId = model.UserId,
			LearningUnitId = model.LearningUnitId,
			OverallBand = (decimal)band,
			TotalCorrect = correctCount,
			TotalQuestions = totalQuestions,
			TimeSpent = model.TimeSpent,
			UserAnswers = resultItems.Select(r => new UserAnswerEntity
			{
				Id = Guid.NewGuid(),
				AttemptId = attemptId,
				QuestionId = r.QuestionId,
				SubQuestionId = r.SubQuestionId,
				UserInput = r.UserAnswer,
				IsCorrect = r.IsCorrect
			}).ToList()
		};

		await _unitOfWork.Repository<UserTestAttemptEntity>().AddAsync(newAttempt);

		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure("Submit failed");

		var result = new SubmissionResult
		{
			TotalQuestions = totalQuestions,
			CorrectCount = correctCount,
			Score = rawScore,
			Band = band,
			Details = resultItems
		};

		return OperationResult.SuccessResult("Submit successfully.", result);
	}
	public async Task<OperationResult> UpdateLearningAsync(Guid id, UpdateListeningForLearningModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Listening))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		var listeningEntity = await _repository.GetByIdAsync(id);

		_mapper.Map(model, listeningEntity);
		listeningEntity.LearningUnit!.SkillType = SkillType.Listening;
		try
		{
			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				_unitOfWork.Repository<ListeningEntity>().UpdateAsync(listeningEntity);
				var result = await _unitOfWork.SaveChangesAsync();
				if (!result)
				{
					throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)));
				}
			});
			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(SpeakingEntity)));
		}
		catch (Exception exception)
		{
			if (model.Media != null)
				await _blobStorageService.DeleteFileByUrlAsync(listeningEntity.Media!.SourceUrl);
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(SpeakingEntity), exception.InnerException?.Message ?? exception.Message));
		}
	}
	public async Task<OperationResult> UpdateIeltsAsync(Guid id, UpdateListeningForIeltsModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Listening))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		var learningUnitRepo = _unitOfWork.Repository<LearningUnitEntity>();
		var existingUnit = await _repository.GetListeningUnitWithQuestionsAsync(id);

		_mapper.Map(model, existingUnit);
		//existingUnit.SkillType = SkillType.Listening;
		//existingUnit.LearningUnitType = LearningUnitType.Ielts;

		//var listeningRepo = _unitOfWork.Repository<ListeningEntity>();
		//var questionRepo = _unitOfWork.Repository<QuestionEntity>();
		//var subQuestionRepo = _unitOfWork.Repository<SubQuestionEntity>();

		//var existingSections = await listeningRepo.GetListByConditionAsync(x => x.LearningUnitId == id);

		//var modelSectionIds = model.Data
		//    .Select(x => x.Media.Id)
		//    .Where(g => g != Guid.Empty)
		//    .ToHashSet();

		//var sectionsToRemove = existingSections
		//    .Where(s => !modelSectionIds.Contains(s.MediaId))
		//    .ToList();

		//if (sectionsToRemove.Any())
		//    listeningRepo.DeleteRangeAsync(sectionsToRemove);

		//foreach (var sectionModel in model.Data)
		//{
		//    ListeningEntity listeningEntity;

		//    if (sectionModel.Media.Id != Guid.Empty)
		//    {
		//        listeningEntity = existingSections.FirstOrDefault(s => s.MediaId == sectionModel.Media.Id)!;
		//        if (listeningEntity != null)
		//        {
		//            listeningEntity.SectionIndex = sectionModel.SectionIndex;
		//            listeningEntity.EstimatedReadingTime = sectionModel.EstimatedReadingTime;
		//            listeningEntity.Media!.SourceUrl = sectionModel.Media.SourceUrl;
		//            listeningEntity.Media.MediaType = MediaType.Audio;
		//        }
		//    }
		//    else
		//    {
		//        listeningEntity = _mapper.Map<ListeningEntity>(sectionModel);
		//        listeningEntity.LearningUnitId = existingUnit.Id;
		//        await listeningRepo.AddAsync(listeningEntity);
		//    }

		//    var existingQuestions = await questionRepo.GetListByConditionAsync(q => q.ModuleItemId == listeningEntity!.Id);
		//    var modelQuestionIds = sectionModel.Questions.Select(q => q.Id).ToHashSet();

		//    var questionsToRemove = existingQuestions.Where(q => !modelQuestionIds.Contains(q.Id)).ToList();
		//    if (questionsToRemove.Any())
		//        questionRepo.DeleteRangeAsync(questionsToRemove);

		//    foreach (var qModel in sectionModel.Questions)
		//    {
		//        QuestionEntity questionEntity;
		//        if (qModel.Id != Guid.Empty)
		//        {
		//            questionEntity = existingQuestions.FirstOrDefault(q => q.Id == qModel.Id)!;
		//            if (questionEntity != null)
		//            {
		//                questionEntity.Prompt = qModel.Prompt ?? questionEntity.Prompt;
		//                //questionEntity.QuestionType = qModel.QuestionType ?? questionEntity.QuestionType;
		//                questionEntity.CorrectAnswer = qModel.CorrectAnswer ?? questionEntity.CorrectAnswer;
		//                //questionEntity.Options = 
		//            }
		//        }
		//        else
		//        {
		//            questionEntity = _mapper.Map<QuestionEntity>(qModel);
		//            questionEntity.Id = Guid.NewGuid();
		//            questionEntity.ModuleItemId = listeningEntity.Id;
		//            questionEntity.ModuleType = LearningModuleType.ListeningLesson;
		//            await questionRepo.AddAsync(questionEntity);
		//        }

		//        // 7️⃣ Xử lý SubQuestions
		//        var existingSubs = await subQuestionRepo.GetListByConditionAsync(s => s.QuestionId == questionEntity.Id);
		//        var modelSubIds = qModel.SubQuestions.Select(s => s.Id).ToHashSet();

		//        var subsToRemove = existingSubs.Where(s => !modelSubIds.Contains(s.Id)).ToList();
		//        if (subsToRemove.Any())
		//            subQuestionRepo.DeleteRangeAsync(subsToRemove);

		//        foreach (var subModel in qModel.SubQuestions)
		//        {
		//            SubQuestionEntity subEntity;
		//            if (subModel.Id != Guid.Empty)
		//            {
		//                subEntity = existingSubs.FirstOrDefault(s => s.Id == subModel.Id)!;
		//                if (subEntity != null)
		//                {
		//                    subEntity.Label = subModel.Label;
		//                    subEntity.Prompt = subModel.Prompt;
		//                    subEntity.CorrectAnswer = subModel.CorrectAnswer;
		//                    subEntity.Options = subModel.Options != null ? string.Join(";", subModel.Options) : null;
		//                }
		//            }
		//            else
		//            {
		//                subEntity = _mapper.Map<SubQuestionEntity>(subModel);
		//                subEntity.Id = Guid.NewGuid();
		//                subEntity.QuestionId = questionEntity.Id;
		//                await subQuestionRepo.AddAsync(subEntity);
		//            }
		//        }
		//    }
		//}


		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ListeningEntity)));

		return OperationResult.SuccessResult(
			ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, $"IELTS Listening ({id})")
		);
	}

	public async Task<OperationResult> DeleteAsync(Guid id)
	{
		await _unitOfWork.Repository<ListeningEntity>().DeleteByIdAsync(id);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(ReadingPassageEntity)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(ReadingPassageEntity)));
	}
	private double ConvertToBand(double correctCount)
	{
		return correctCount switch
		{
			>= 39 => 9.0,
			>= 37 => 8.5,
			>= 35 => 8.0,
			>= 33 => 7.5,
			>= 30 => 7.0,
			>= 27 => 6.5,
			>= 23 => 6.0,
			>= 20 => 5.5,
			>= 16 => 5.0,
			>= 13 => 4.5,
			>= 10 => 4.0,
			>= 7 => 3.5,
			>= 5 => 3.0,
			>= 3 => 2.5,
			_ => 0
		};
	}
}