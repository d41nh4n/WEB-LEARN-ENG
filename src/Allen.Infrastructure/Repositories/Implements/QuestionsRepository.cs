namespace Allen.Infrastructure;

[RegisterService(typeof(IQuestionsRepository))]
public class QuestionsRepository(SqlApplicationDbContext dbContext) : RepositoryBase<QuestionEntity>(dbContext), IQuestionsRepository
{
	private readonly SqlApplicationDbContext _context = dbContext;

	public async Task<QueryResult<QuestionModel>> GetQuestionsAsync(Guid moduleItemId, QueryInfo queryInfo)
	{
		var baseQuery = _context.Questions
			.AsNoTracking()
			.Include(q => q.SubQuestions)
			.Where(q => q.ModuleItemId == moduleItemId);

		var totalCount = queryInfo.NeedTotalCount
			? await baseQuery.CountAsync()
			: 0;

		var rawQuestions = await baseQuery
			.OrderBy(q => q.Label)
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.Select(q => new
			{
				q.Id,
				q.ModuleItemId,
				q.QuestionType,
				q.Prompt,
				q.Options,
				q.CorrectAnswer,
				q.ContentUrl,
				q.Label,
				q.TableMetadata,
				q.SubQuestions
			})
			.ToListAsync();

		var entities = rawQuestions.Select(q => new QuestionModel
		{
			Id = q.Id,
			ModuleItemId = q.ModuleItemId,
			QuestionType = q.QuestionType.ToString(),
			Prompt = q.Prompt,
			Options = JsonHelper.Deserialize<List<string>>(q.Options),
			CorrectAnswer = q.CorrectAnswer,
			ContentUrl = q.ContentUrl,
			Label = q.Label,
			TableMetadata = JsonHelper.Deserialize<Table>(q.TableMetadata),
			SubQuestions = q.SubQuestions?
			.Select(sub => new SubQuestionModel
			{
				Id = sub.Id,
				QuestionId = sub.QuestionId,
				Label = sub.Label ?? "",
				Prompt = sub.Prompt,
				CorrectAnswer = sub.CorrectAnswer,
				Options = JsonHelper.Deserialize<List<string>>(sub.Options)
			})
			.ToList() ?? new List<SubQuestionModel>()
		}).ToList();

		return new QueryResult<QuestionModel>
		{
			Data = entities,
			TotalCount = totalCount
		};
	}

	public async Task<QuestionModel> GetQuestionByIdAsync(Guid id)
	{
		var questionEntity = await _context.Questions
			.AsNoTracking()
			.Include(q => q.SubQuestions)
			.FirstOrDefaultAsync(q => q.Id == id);

		if (questionEntity == null)
			throw new NotFoundException(
				ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(QuestionModel), id)
			);

		var question = new QuestionModel
		{
			Id = questionEntity.Id,
			QuestionType = questionEntity.QuestionType.ToString(),
			ModuleItemId = questionEntity.ModuleItemId,
			Prompt = questionEntity.Prompt,
			Options = JsonHelper.Deserialize<List<string>>(questionEntity.Options),
			CorrectAnswer = questionEntity.CorrectAnswer,
			ContentUrl = questionEntity.ContentUrl,
			Label = questionEntity.Label,
			TableMetadata = JsonHelper.Deserialize<Table>(questionEntity.TableMetadata),
			SubQuestions = questionEntity.SubQuestions?
			.Select(sub => new SubQuestionModel
			{
				Id = sub.Id,
				QuestionId = sub.QuestionId,
				Label = sub.Label ?? "",
				Prompt = sub.Prompt,
				CorrectAnswer = sub.CorrectAnswer,
				Options = JsonHelper.Deserialize<List<string>>(sub.Options)
			})
			.ToList() ?? new List<SubQuestionModel>()
		};

		return question;
	}

	public async Task<List<QuestionEntity>> GetQuestionsByLearningUnitIdAsync(Guid learningUnitId, bool readingOrListening)
	{
		List<Guid> ids;
		if (readingOrListening)
		{
			ids = await _context.ReadingPassages.AsNoTracking()
						.Where(rp => rp.LearningUnitId == learningUnitId)
						.Select(rp => rp.Id)
						.ToListAsync();
		}
		else
		{
			ids = await _context.Listenings.AsNoTracking()
						.Where(rp => rp.LearningUnitId == learningUnitId)
						.Select(rp => rp.Id)
						.ToListAsync();
		}
		

		return await _context.Questions
			.Include(q => q.SubQuestions)
			.AsNoTracking()
			.Where(q => ids.Contains(q.ModuleItemId))
			.ToListAsync();
	}

	public async Task<List<SubQuestionEntity>> GetSubQuestionsByLearningUnitIdAsync(Guid learningUnitId, bool readingOrListening)
	{
		List<Guid> ids;
		if (readingOrListening)
		{
			ids = await _context.ReadingPassages.AsNoTracking()
						.Where(rp => rp.LearningUnitId == learningUnitId)
						.Select(rp => rp.Id)
						.ToListAsync();
		}
		else
		{
			ids = await _context.Listenings.AsNoTracking()
						.Where(rp => rp.LearningUnitId == learningUnitId)
						.Select(rp => rp.Id)
						.ToListAsync();
		}

		return await (from subQuestion in _context.SubQuestions.AsNoTracking()
					  join question in _context.Questions.AsNoTracking()
						  on subQuestion.QuestionId equals question.Id
					  where ids.Contains(question.ModuleItemId)
					  select subQuestion)
					 .ToListAsync();
	}

	public async Task<List<AnswerResult>> GetAnswersOfUserByLearningIdAsync(Guid learningUnitId, Guid userId)
	{
		var query = from uta in _context.UserTestAttempts.AsNoTracking()
					join ua in _context.UserAnswers.AsNoTracking()
						on uta.Id equals ua.AttemptId
					where uta.UserId == userId
						  && uta.LearningUnitId == learningUnitId
					select new AnswerResult
					{
						QuestionId = ua.QuestionId,
						SubQuestionId = ua.SubQuestionId,
						UserAnswer = ua.UserInput,
						IsCorrect = ua.IsCorrect ?? false,
					};

		return await query.ToListAsync();
	}
}
