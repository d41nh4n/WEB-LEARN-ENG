namespace Allen.Infrastructure;

[RegisterService(typeof(IListeningsRepository))]
public class ListeningsRepository(SqlApplicationDbContext context) : RepositoryBase<ListeningEntity>(context), IListeningsRepository
{
	private readonly SqlApplicationDbContext _context = context;

	public async Task<ListeningEntity> GetById(Guid id)
		=> await (from listenting in _context.Listenings
				  join unit in _context.LearningUnits
					  on listenting.LearningUnitId equals unit.Id into unitJoin
				  from unit in unitJoin.DefaultIfEmpty()
				  where listenting.Id == id
				  select new ListeningEntity
				  {
					  Id = listenting.Id,
					  LearningUnitId = listenting.LearningUnitId,
					  LearningUnit = unit == null ? null : new LearningUnitEntity
					  {
						  Id = unit.Id,
						  CategoryId = unit.CategoryId,
						  Description = unit.Description,
						  SkillType = unit.SkillType,
						  Level = unit.Level,
						  CreatedAt = unit.CreatedAt,
						  LastModified = unit.LastModified
					  }
				  }).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ListeningEntity), id));

	public async Task<ListeningModel> GetByLearningUnitId(Guid learningUnitId)
	{
		return await (from listening in _context.Listenings.AsNoTracking()
					  join media in _context.Medias.AsNoTracking() on listening.MediaId equals media.Id
					  join TranscriptEntity transcript in _context.Transcripts.AsNoTracking() on media.Id equals transcript.MediaId into transcriptGroup
					  where listening.LearningUnitId == learningUnitId
					  select new ListeningModel
					  {
						  Id = listening.Id,
						  LearningUnitId = listening.LearningUnitId,
						  Media = new MediaModel
						  {
							  Id = media.Id,
							  Title = media.Title,
							  MediaType = media.MediaType.ToString(),
							  SourceUrl = media.SourceUrl,
							  Transcripts = new QueryResult<TranscriptModel>
							  {
								  Data = transcriptGroup.Select(t => new TranscriptModel
								  {
									  Id = t.Id,
									  ContentEN = t.ContentEN,
									  ContentVN = t.ContentVN,
									  StartTime = t.StartTime,
									  EndTime = t.EndTime,
									  IPA = t.IPA,
									  OrderIndex = t.OrderIndex
								  }).ToList(),
								  TotalCount = transcriptGroup.Count()
							  }
						  }
					  }).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ListeningEntity), learningUnitId));
	}

	public async Task<ListeningForIeltsModel> GetByLearningUnitIdForIeltsAsync(
	Guid learningUnitId,
	GetByLearningUnitIdForIeltsQuery query)
	{
		var listeningQuery = _context.Listenings
			.AsNoTracking()
			.Where(l => l.LearningUnitId == learningUnitId)
			.OrderBy(l => l.SectionIndex)
			.AsQueryable();

		if (query.SectionIndex is { Count: > 0 })
		{
			listeningQuery = listeningQuery.Where(l =>
				l.SectionIndex.HasValue &&
				query.SectionIndex.Contains(l.SectionIndex.Value)
			);
		}

		var listeningEntities = await listeningQuery.ToListAsync();

		if (!listeningEntities.Any())
		{
			return new ListeningForIeltsModel();
		}

		var listeningIds = listeningEntities.Select(l => l.Id).ToList();

		var questions = await _context.Questions
			.AsNoTracking()
			.Include(q => q.SubQuestions)
			.Where(q => listeningIds.Contains(q.ModuleItemId))
			.OrderBy(q => q.CreatedAt)
			.ToListAsync();

		var mediaIds = listeningEntities
			.Where(l => l.MediaId != Guid.Empty)
			.Select(l => l.MediaId)
			.ToList();

		var medias = await _context.Medias
			.AsNoTracking()
			.Where(m => mediaIds.Contains(m.Id))
			.ToListAsync();

		var unitTitle = await _context.LearningUnits
			.Where(u => u.Id == learningUnitId)
			.Select(u => u.Title)
			.FirstOrDefaultAsync() ?? string.Empty;

		var result = new ListeningForIeltsModel
		{
			LearningUnitId = learningUnitId,
			Title = unitTitle,
			EstimatedReadingTime = listeningEntities.FirstOrDefault()?.EstimatedReadingTime ?? 0,
			Data = listeningEntities
				.OrderBy(l => l.SectionIndex)
				.Select(l => new GetListeningSectionModel
				{
					ListeningId = l.Id,
					SectionIndex = l.SectionIndex ?? 0,
					Media = medias
						.Where(m => m.Id == l.MediaId)
						.Select(m => new GetMediaWithoutTranscriptModel
						{
							Id = m.Id,
							Title = m.Title,
							MediaType = m.MediaType.ToString(),
							SourceUrl = m.SourceUrl
						})
						.FirstOrDefault() ?? new GetMediaWithoutTranscriptModel(),
					Questions = questions
						.Where(q => q.ModuleItemId == l.Id)
						.OrderBy(q => q.CreatedAt)
						.Select(q => new QuestionModel
						{
							Id = q.Id,
							QuestionType = q.QuestionType.ToString(),
							ModuleItemId = q.ModuleItemId,
							Label = q.Label,
							Prompt = q.Prompt,
							Options = JsonHelper.Deserialize<List<string>>(q.Options),
							CorrectAnswer = q.CorrectAnswer,
							ContentUrl = q.ContentUrl,
							TableMetadata = JsonHelper.Deserialize<Table>(q.TableMetadata),
							SubQuestions = q.SubQuestions?
								.OrderBy(s => s.CreatedAt)
								.Select(sub => new SubQuestionModel
								{
									Id = sub.Id,
									QuestionId = sub.QuestionId,
									Label = sub.Label ?? "",
									Prompt = sub.Prompt,
									CorrectAnswer = sub.CorrectAnswer,
									Options = JsonHelper.Deserialize<List<string>>(sub.Options)
								}).ToList() ?? new List<SubQuestionModel>()
						})
						.ToList()
				})
				.ToList()
		};
		return result;
	}

	public async Task<LearningUnitEntity> GetListeningUnitWithQuestionsAsync(Guid id)
	{
		var learningUnit = await _context.LearningUnits
			.Include(u => u.Listenings)
				.ThenInclude(l => l.Media)
			.FirstOrDefaultAsync(u => u.Id == id);

		if (learningUnit == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, id));

		var listeningIds = learningUnit.Listenings.Select(l => l.Id).ToList();

		var questions = await _context.Set<QuestionEntity>()
			.Where(q => listeningIds.Contains(q.ModuleItemId))
			.Include(q => q.SubQuestions)
			.ToListAsync();

		var questionsByListening = questions
			.GroupBy(q => q.ModuleItemId)
			.ToDictionary(g => g.Key, g => g.ToList());

		foreach (var listening in learningUnit.Listenings)
		{
			listening.Questions = questionsByListening.TryGetValue(listening.Id, out var qs)
				? qs
				: new List<QuestionEntity>();
		}

		return learningUnit;
	}
}
