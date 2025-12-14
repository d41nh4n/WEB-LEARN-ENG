namespace Allen.Application;

[RegisterService(typeof(IReadingPassagesService))]

public class ReadingPassagesService(
	IReadingPassagesRepository _repository,
	IQuestionsRepository _questionRepository,
	IUnitOfWork _unitOfWork,
	IMapper _mapper) : IReadingPassagesService
{
	public async Task<QueryResult<ReadingPassageModel>> GetByUnitIdAsync(Guid unitId, QueryInfo queryInfo)
		=> await _repository.GetByUnitIdAsync(unitId, queryInfo);
	public async Task<ReadingPassageModel> GetByIdAsync(Guid id)
	   => await _repository.GetQuestionByIdAsync(id);
	public async Task<ReadingPassageForLearningModel> GetLearningByIdAsync(Guid id)
	   => await _repository.GetLearningByIdAsync(id);
	public async Task<List<ReadingPassageForIeltsModel>> GetLearningByIdForIeltsAsync(Guid id, ReadingPassageQuery query)
	   => await _repository.GetLearningByIdForIeltsAsync(id, query);

	public Task<ReadingPassageSumaryModel> GetSumaryByUnitIdAsync(Guid learningUnitId)
	   => _repository.GetSumaryByUnitIdAsync(learningUnitId);
	public async Task<OperationResult> CreateIeltsReadingPassageAsync(CreateIeltsReadingPassageModel model)
	{
		if (!await _unitOfWork.Repository<LearningUnitEntity>().CheckExistAsync(x => x.Id == model.LearningUnitId))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(LearningUnitEntity), model.LearningUnitId));

		var existingSections = await _unitOfWork.Repository<ReadingPassageEntity>()
			.GetListByConditionAsync(x => x.LearningUnitId == model.LearningUnitId);

		if (!existingSections.Any() && model.ReadingPassageNumber != 1)
		{
			return OperationResult.Failure("The first Reading section must have SectionIndex = 1.");
		}

		if (existingSections.Any(x => x.ReadingPassageNumber == model.ReadingPassageNumber))
		{
			return OperationResult.Failure($"ReadingPassageNumber {model.ReadingPassageNumber} already exists.");
		}

		var entity = _mapper.Map<ReadingPassageEntity>(model);
		await _unitOfWork.Repository<ReadingPassageEntity>().AddAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ReadingPassageEntity)));
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ReadingPassageEntity)), entity.Id);
	}
	public async Task<OperationResult> CreateIeltsReadingPassagesAsync(CreateIeltsReadingPassagesModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Reading))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		var entity = _mapper.Map<ReadingPassageEntity>(model);
		entity.LearningUnit.SkillType = SkillType.Reading;
		entity.LearningUnit.LearningUnitType = LearningUnitType.Ielts;
		entity.LearningUnit!.LearningUnitStatusType = LearningUnitStatusType.Draft;


		await _unitOfWork.Repository<ReadingPassageEntity>().AddAsync(entity);

		foreach (var item in entity.Questions)
		{
			item.ModuleItemId = entity.Id;
			item.ModuleType = LearningModuleType.ReadingPassage;
		}
		await _unitOfWork.Repository<QuestionEntity>().AddRangeAsync(entity.Questions);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ReadingPassageEntity)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ReadingPassageEntity)));
	}

	public async Task<OperationResult> CreateLearningReadingAsync(CreateLearningReadingPassageModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId && x.SkillType == SkillType.Reading))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));

		var entity = _mapper.Map<ReadingPassageEntity>(model);
		entity.LearningUnit.SkillType = SkillType.Reading;
		entity.LearningUnit.LearningUnitType = LearningUnitType.Academy;

		int i = 0;
		foreach (var item in entity.Paragraphs)
		{
			item.Order = i++;
		}

		await _unitOfWork.Repository<ReadingPassageEntity>().AddAsync(entity);

		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ReadingPassageEntity)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ReadingPassageEntity)));
	}


	public async Task<OperationResult> UpdateAsync(Guid id, UpdateReadingPassagesModel model)
	{
		if (!await _unitOfWork.Repository<ReadingPassageEntity>().CheckExistAsync(x => x.Id == id))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ReadingPassageEntity), id));

		if (await _unitOfWork.Repository<ReadingPassageEntity>().CheckExistAsync(x => x.Id != id && x.ReadingPassageNumber == model.ReadingPassageNumber))
			return OperationResult.Failure($"Reading passage number {model.ReadingPassageNumber} already exists");

		var entity = await _unitOfWork.Repository<ReadingPassageEntity>().GetByIdAsync(id);
		_mapper.Map(model, entity);

		_unitOfWork.Repository<ReadingPassageEntity>().UpdateAsync(entity);
		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ReadingPassageEntity)));
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ReadingPassageEntity)));
	}
	public async Task<OperationResult> SubmitIeltsReadingAsync(SubmitIeltsModel model)
	{
		var questions = await _questionRepository.GetQuestionsByLearningUnitIdAsync(model.LearningUnitId, true);
		var subQuestions = await _questionRepository.GetSubQuestionsByLearningUnitIdAsync(model.LearningUnitId, true);

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

		double rawScore = totalQuestions == 0 ? 0 : (double)correctCount / totalQuestions * 40;
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

	public async Task<OperationResult> UpdateLearningAsync(Guid id, UpdateReadingPassageForLearningModel model)
	{
		if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId &&x.SkillType == SkillType.Reading))
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));
		}

		var entity = await _repository.GetByIdForUpdate(id);

		if (entity == null)
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ReadingPassageEntity), id));
		
		_mapper.Map(model, entity);
		_mapper.Map(model.LearningUnit, entity.LearningUnit!);


		var readingParagraphEntities = await _unitOfWork.Repository<ReadingParagraphEntity>().GetListByConditionAsync(x => x.PassageId == id);

		var readingParagraphsInDbDict = readingParagraphEntities.ToDictionary(q => q.Id, q => q);

		var modelParagraphIds = new HashSet<Guid>();

		foreach (var paragraphModel in model.Paragraphs)
		{
			if (paragraphModel.Id.HasValue &&
				readingParagraphsInDbDict.TryGetValue(paragraphModel.Id.Value, out var existingEntity))
			{
				_mapper.Map(paragraphModel, existingEntity);
				modelParagraphIds.Add(existingEntity.Id);
			}
			else
			{
				var newEntity = _mapper.Map<ReadingParagraphEntity>(paragraphModel);
				newEntity.PassageId = id;
				await _unitOfWork.Repository<ReadingParagraphEntity>().AddAsync(newEntity);
			}
		}

		var paragraphsToDelete = readingParagraphEntities
			.Where(p => !modelParagraphIds.Contains(p.Id))
			.ToList();

		if (paragraphsToDelete.Any())
		{
			 _unitOfWork.Repository<ReadingParagraphEntity>().DeleteRangeAsync(paragraphsToDelete);
		}

		 _unitOfWork.Repository<ReadingPassageEntity>().UpdateAsync(entity);

		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ReadingPassageEntity)));
		}

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ReadingPassageEntity)));
	}

	public async Task<OperationResult> DeleteAsync(Guid id)
	{
		await _unitOfWork.Repository<ReadingPassageEntity>().DeleteByIdAsync(id);
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
