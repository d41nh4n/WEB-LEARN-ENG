namespace Allen.Infrastructure;
[RegisterService(typeof(IReadingPassagesRepository))]
public class ReadingPassagesRepository(SqlApplicationDbContext context) : RepositoryBase<ReadingPassageEntity>(context), IReadingPassagesRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<ReadingPassageEntity> GetByIdForUpdate(Guid id)
    {
        return await _context.ReadingPassages
            .Include(rp => rp.Paragraphs)
            .Select(rp => new ReadingPassageEntity
            {
                Id = rp.Id,
                LearningUnitId = rp.LearningUnitId,
                LearningUnit = rp.LearningUnit,
				Title = rp.Title,
                Content = rp.Content,
                EstimatedReadingTime = rp.EstimatedReadingTime,
                Paragraphs = rp.Paragraphs.OrderBy(p => p.Order).ToList()
            })
            .FirstOrDefaultAsync(rp => rp.Id == id)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ReadingParagraphEntity), id));
    }

    public async Task<QueryResult<ReadingPassageModel>> GetByUnitIdAsync(Guid unitId, QueryInfo queryInfo)
    {
        var query = from passage in _context.ReadingPassages
                    where passage.LearningUnitId == unitId
                    join question in _context.Questions
                        on passage.Id equals question.ModuleItemId into passageQuestions
                    select new ReadingPassageModel
                    {
                        Id = passage.Id,
                        Title = passage.Title ?? "",
                        Content = passage.Content ?? "",
                        LearningUnitId = passage.LearningUnitId,
                        CreatedAt = passage.CreatedAt,

                        GroupedQuestions = passageQuestions
                            .Where(q => q.ModuleType == LearningModuleType.ReadingPassage)
                            .GroupBy(q => q.QuestionType)
                            .Select(g => new QuestionGroupModel
                            {
                                QuestionType = g.Key.ToString(),
                                Questions = g.Select(q => new QuestionModel
                                {
                                    Id = q.Id,
                                    ModuleItemId = q.ModuleItemId,
                                    QuestionType = q.QuestionType.ToString(),
                                    Prompt = q.Prompt,
                                    //Options = q.Options,
                                    CorrectAnswer = q.CorrectAnswer,
                                    ContentUrl = q.ContentUrl
                                }).ToList()
                            }).ToList()
                    };

        var totalItems = queryInfo.NeedTotalCount
            ? await query.CountAsync()
            : 0;

        var entities = await query
            .OrderBy(x => x.CreatedAt)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<ReadingPassageModel>
        {
            Data = entities,
            TotalCount = totalItems,
        };
    }

    public async Task<ReadingPassageForLearningModel> GetLearningByIdAsync(Guid id)
    {
        var entity = await _context.ReadingPassages.AsNoTracking()
            .Include(rp => rp.LearningUnit)
            .Include(rp => rp.Paragraphs)
            .Select(rp => new ReadingPassageForLearningModel()
            {
                Id = rp.Id,
                LearningUnitId = rp.LearningUnitId,
                CategoryId = rp.LearningUnit.CategoryId ?? Guid.Empty,
                Title = rp.LearningUnit.Title ?? "",
                Description = rp.LearningUnit.Description,
                Level = rp.LearningUnit.Level.ToString(),
                Paragraphs = rp.Paragraphs.Select(p => new ReadingParagraphModel
                {
                    Id = p.Id,
                    Order = p.Order,
                    OriginalText = p.OriginalText,
                    Transcript = p.Transcript
                }).ToList(),

            })
            .FirstOrDefaultAsync(rp => rp.LearningUnitId == id)
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ReadingPassageEntity), id));

        return entity;
    }

    public async Task<List<ReadingPassageForIeltsModel>> GetLearningByIdForIeltsAsync(Guid id, ReadingPassageQuery query)
    {
        var passageQuery = _context.ReadingPassages
            .AsNoTracking()
            .Where(p => p.LearningUnitId == id)
            .OrderBy(p => p.ReadingPassageNumber)
            .AsQueryable();

        if (query.ReadingPassageNumber is { Count: > 0 })
        {
            passageQuery = passageQuery.Where(p =>
                p.ReadingPassageNumber.HasValue &&
                query.ReadingPassageNumber.Contains(p.ReadingPassageNumber.Value)
            );
        }

        var passages = await passageQuery.ToListAsync();

        if (!passages.Any())
        {
            return new List<ReadingPassageForIeltsModel>();
		};

        var passageIds = passages.Select(p => p.Id).ToList();

        var questions = await _context.Questions
            .AsNoTracking()
            .Include(q => q.SubQuestions)
            .Where(q => passageIds.Contains(q.ModuleItemId))
            .OrderBy(q => q.Label)
            .ToListAsync();

        var result = passages
            .OrderBy(p => p.ReadingPassageNumber)
            .Select(p => new ReadingPassageForIeltsModel
            {
                Id = p.Id,
				LearningUnitId = p.LearningUnitId,
                Title = p.Title,
                Content = p.Content,
                EstimatedReadingTime = p.EstimatedReadingTime,
                ReadingPassageNumber = p.ReadingPassageNumber,
                Questions = questions
                    .OrderBy(q => q.CreatedAt)
                    .Where(q => q.ModuleItemId == p.Id)
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
            .ToList();

        return result;
    }

    public async Task<ReadingPassageModel> GetQuestionByIdAsync(Guid id)
    {
        return await (from passage in _context.ReadingPassages.AsNoTracking()
                      where passage.Id == id
                      join question in _context.Questions.AsNoTracking()
                          on passage.Id equals question.ModuleItemId into passageQuestions
                      select new ReadingPassageModel
                      {
                          Id = passage.Id,
                          Title = passage.Title ?? "",
                          Content = passage.Content ?? "",
                          LearningUnitId = passage.LearningUnitId,
                          CreatedAt = passage.CreatedAt,

                          GroupedQuestions = passageQuestions
                              .Where(q => q.ModuleType == LearningModuleType.ReadingPassage)
                              .GroupBy(q => q.QuestionType)
                              .Select(g => new QuestionGroupModel
                              {
                                  QuestionType = g.Key.ToString(),
                                  Questions = g.Select(q => new QuestionModel
                                  {
                                      Id = q.Id,
                                      ModuleItemId = q.ModuleItemId,
                                      QuestionType = q.QuestionType.ToString(),
                                      Prompt = q.Prompt,
                                      //Options = q.Options,
                                      CorrectAnswer = q.CorrectAnswer,
                                      ContentUrl = q.ContentUrl
                                  }).ToList()
                              }).ToList()
                      }).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ReadingPassageEntity), id));
    }

    public async Task<ReadingPassageSumaryModel> GetSumaryByUnitIdAsync(Guid learningUnitId)
    {
        return await (from passage in _context.ReadingPassages.AsNoTracking()
                      join unit in _context.Questions.AsNoTracking()
                       on passage.LearningUnitId equals unit.ModuleItemId into questionsGroup
                      where passage.LearningUnitId == learningUnitId
                      select new ReadingPassageSumaryModel
                      {
                          Id = passage.Id,
                          Title = passage.Title ?? "",
                          EstimatedReadingTime = passage.EstimatedReadingTime,
                          TotalQuestions = questionsGroup.Count()
                      })
                    .FirstOrDefaultAsync()
                    ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ReadingPassageEntity), learningUnitId));
    }
}
