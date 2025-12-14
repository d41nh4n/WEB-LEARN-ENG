namespace Allen.Infrastructure;

[RegisterService(typeof(IWritingRepository))]
public class WritingRepository(
    SqlApplicationDbContext context
) : RepositoryBase<WritingEntity>(context), IWritingRepository
{
    private readonly SqlApplicationDbContext _context = context;

    // -- Learning --
    public async Task<WritingLearningModel> GetLearningWritingByIdAsync(Guid id)
    {
        return await _context.Writings
            .AsNoTracking()
            .Where(w => w.Id == id && w.LearningUnit.LearningUnitType == LearningUnitType.Academy)
            .Select(w => new WritingLearningModel
            {
                Id = w.Id,
                LearningUnitId = w.LearningUnitId,
                ContentEN = w.ContentEN ?? "",
                ContentVN = w.ContentVN ?? "",
                LearningUnit = new CreateLearningUnitForWritingModel
                {
                    CategoryId = w.LearningUnit.CategoryId,
                    Title = w.LearningUnit.Title,
                    Description = w.LearningUnit.Description,
                    Level = w.LearningUnit.Level.ToString()
                }
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(WritingEntity), id));
    }

    public async Task<WritingLearningModel> GetLearningWritingByLearningUnitIdAsync(Guid id)
    {
        return await _context.Writings
            .Where(w => w.LearningUnitId == id && w.LearningUnit.LearningUnitType == LearningUnitType.Academy)
            .Select(w => new WritingLearningModel
            {
                Id = w.Id,
                LearningUnitId = w.LearningUnitId,
                ContentEN = w.ContentEN ?? "",
                ContentVN = w.ContentVN ?? "",
                LearningUnit = new CreateLearningUnitForWritingModel
                {
                    CategoryId = w.LearningUnit.CategoryId,
                    Title = w.LearningUnit.Title,
                    Description = w.LearningUnit.Description,
                    Level = w.LearningUnit.Level.ToString()
                }
            }).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(WritingEntity), id));
    }

    public async Task<QueryResult<WritingLearningModel>> GetLearningWritingsAsync(QueryInfo queryInfo)
    {
        var query = _context.Writings
            .AsNoTracking()
            .Where(x => x.LearningUnit.LearningUnitType == LearningUnitType.Academy);

        if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
        {
            var search = queryInfo.SearchText.Trim();
            query = query.Where(w =>
                EF.Functions.Collate(w.ContentVN ?? "", "Latin1_General_CI_AI").Contains(search) ||
                EF.Functions.Collate(w.ContentEN ?? "", "Latin1_General_CI_AI").Contains(search) ||
                EF.Functions.Collate(w.LearningUnit.Title, "Latin1_General_CI_AI").Contains(search));
        }

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var orderedQuery = string.IsNullOrEmpty(queryInfo.OrderBy)
            ? query.OrderBy(w => w.LearningUnit.Title) // sort alphabet A → Z
            : queryInfo.OrderType == OrderType.Ascending
                ? query.OrderBy(e => EF.Property<object>(e, queryInfo.OrderBy))
                : query.OrderByDescending(e => EF.Property<object>(e, queryInfo.OrderBy));

        var writings = await orderedQuery
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(w => new WritingLearningModel
            {
                Id = w.Id,
                LearningUnitId = w.LearningUnitId,
                ContentEN = w.ContentEN ?? "",
                ContentVN = w.ContentVN ?? "",
                LearningUnit = new CreateLearningUnitForWritingModel
                {
                    CategoryId = w.LearningUnit.CategoryId,
                    Title = w.LearningUnit.Title,
                    Description = w.LearningUnit.Description,
                    Level = w.LearningUnit.Level.ToString()
                }
            })
            .ToListAsync();

        return new QueryResult<WritingLearningModel>
        {
            Data = writings,
            TotalCount = total
        };
    }

    // -- Ielts --
    public async Task<WritingIeltsModel> GetIeltsWritingByIdAsync(Guid id)
    {
        return await _context.Writings
            .AsNoTracking()
            .Where(w => w.Id == id && w.LearningUnit.LearningUnitType == LearningUnitType.Ielts)
            .Select(w => new WritingIeltsModel
            {
                Id = w.Id,
                LearningUnitId = w.LearningUnitId,
                TaskType = w.TaskType.ToString(),
                ContentEN = w.ContentEN ?? "",
                SourceUrl = w.SourceUrl,
                Hint = w.Hint,
                LearningUnit = new CreateLearningUnitForWritingModel
                {
                    CategoryId = w.LearningUnit.CategoryId,
                    Title = w.LearningUnit.Title,
                    Description = w.LearningUnit.Description,
                    Level = w.LearningUnit.Level.ToString()
                }
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(WritingEntity), id));
    }

    public async Task<WritingIeltsModel> GetIeltsWritingByLearningUnitIdAsync(Guid id)
    {
        return await _context.Writings
            .Where(w => w.LearningUnitId == id && w.LearningUnit.LearningUnitType == LearningUnitType.Ielts)
            .Select(w => new WritingIeltsModel
            {
                Id = w.Id,
                LearningUnitId = w.LearningUnitId,
                TaskType = w.TaskType.ToString(),
                ContentEN = w.ContentEN ?? "",
                SourceUrl = w.SourceUrl,
                Hint = w.Hint,
                LearningUnit = new CreateLearningUnitForWritingModel
                {
                    CategoryId = w.LearningUnit.CategoryId,
                    Title = w.LearningUnit.Title,
                    Description = w.LearningUnit.Description,
                    Level = w.LearningUnit.Level.ToString()
                }
            }).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(WritingEntity), id));
    }

    public async Task<QueryResult<WritingIeltsModel>> GetIeltsWritingsAsync(QueryInfo queryInfo)
    {
        var query = _context.Writings
            .AsNoTracking()
            .Where(x => x.LearningUnit.LearningUnitType == LearningUnitType.Ielts);

        if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
        {
            var search = queryInfo.SearchText.Trim();
            query = query.Where(w =>
                EF.Functions.Collate(w.Hint ?? "", "Latin1_General_CI_AI").Contains(search) ||
                EF.Functions.Collate(w.ContentEN ?? "", "Latin1_General_CI_AI").Contains(search) ||
                EF.Functions.Collate(w.LearningUnit.Title, "Latin1_General_CI_AI").Contains(search));
        }

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var orderedQuery = string.IsNullOrEmpty(queryInfo.OrderBy)
            ? query.OrderBy(w => w.LearningUnit.Title)
            : queryInfo.OrderType == OrderType.Ascending
                ? query.OrderBy(e => EF.Property<object>(e, queryInfo.OrderBy))
                : query.OrderByDescending(e => EF.Property<object>(e, queryInfo.OrderBy));

        var writings = await orderedQuery
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(w => new WritingIeltsModel
            {
                Id = w.Id,
                LearningUnitId = w.LearningUnitId,
                TaskType = w.TaskType.ToString(),
                ContentEN = w.ContentEN ?? "",
                SourceUrl = w.SourceUrl,
                Hint = w.Hint,
                LearningUnit = new CreateLearningUnitForWritingModel
                {
                    CategoryId = w.LearningUnit.CategoryId,
                    Title = w.LearningUnit.Title,
                    Description = w.LearningUnit.Description,
                    Level = w.LearningUnit.Level.ToString()
                }
            })
            .ToListAsync();

        return new QueryResult<WritingIeltsModel>
        {
            Data = writings,
            TotalCount = total
        };
    }
}