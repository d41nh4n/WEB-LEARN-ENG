namespace Allen.Infrastructure;

[RegisterService(typeof(ILearningUnitsRepository))]
public class LearningUnitsRepository(SqlApplicationDbContext context) : RepositoryBase<LearningUnitEntity>(context), ILearningUnitsRepository
{
	private readonly SqlApplicationDbContext _context = context;

	public async Task<QueryResult<LearningUnit>> GetAllWithPagingAsync(QueryInfo queryInfo)
	{
		var query = from learningUnit in _context.LearningUnits.AsNoTracking()
					select new LearningUnit
					{
						Id = learningUnit.Id,
						CategoryId = learningUnit.CategoryId,
						Title = learningUnit.Title,
						Level = learningUnit.Level.ToString(),
						SkillType = learningUnit.SkillType.ToString(),
						Description = learningUnit.Description,
					};


		var entities = await query
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.ToListAsync();

		var totalCount = 0;
		if (queryInfo.NeedTotalCount)
		{
			totalCount = await query.CountAsync();
		}
		return new QueryResult<LearningUnit>
		{
			Data = entities,
			TotalCount = totalCount
		};
	}

	public async Task<QueryResult<LearningUnit>> GetByCategoryIdAsync(Guid categoryId, QueryInfo queryInfo)
	{
		var query = from learningUnit in _context.LearningUnits.AsNoTracking()
					where learningUnit.CategoryId == categoryId
					select new LearningUnit
					{
						Id = learningUnit.Id,
						Title = learningUnit.Title,
						Level = learningUnit.Level.ToString(),
						SkillType = learningUnit.SkillType.ToString(),
						Description = learningUnit.Description
					};


		var entities = await query
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.ToListAsync();

		var totalCount = 0;
		if (queryInfo.NeedTotalCount)
		{
			totalCount = await query.CountAsync();
		}
		return new QueryResult<LearningUnit>
		{
			Data = entities,
			TotalCount = totalCount
		};
	}

	public async Task<QueryResult<LearningUnit>> GetByFiltersAsync(LearningUnitQuery learningUnitQuery, QueryInfo queryInfo)
	{
		var query = _context.LearningUnits.AsNoTracking()
			.AsQueryable();

		if (learningUnitQuery.CategoryId.HasValue)
		{
			query = query.Where(x => x.CategoryId == learningUnitQuery.CategoryId);
		}

		if (!string.IsNullOrEmpty(learningUnitQuery.SkillType))
		{
			if (Enum.TryParse(learningUnitQuery.SkillType, out SkillType skillType))
			{
				query = query.Where(x => x.SkillType == skillType);
			}
		}

		if (!string.IsNullOrEmpty(learningUnitQuery.LevelType))
		{
			if (Enum.TryParse(learningUnitQuery.LevelType, out LevelType levelType))
			{
				query = query.Where(x => x.Level == levelType);
			}
		}

		if (!string.IsNullOrEmpty(learningUnitQuery.LearningUnitType))
		{
			if (Enum.TryParse(learningUnitQuery.LearningUnitType, out LearningUnitType unitType))
			{
				query = query.Where(x => x.LearningUnitType == unitType);
			}
		}

		if (!string.IsNullOrEmpty(learningUnitQuery.LearningUnitStatusType))
		{
			if (Enum.TryParse(learningUnitQuery.LearningUnitStatusType, out LearningUnitStatusType unitStatusType))
			{
				query = query.Where(x => x.LearningUnitStatusType == unitStatusType);
			}
		}

		var queryWithSteps = from learningUnit in query
							 select new
							 {
								 learningUnit
							 };

		if (!string.IsNullOrEmpty(learningUnitQuery.TaskType))
		{
			if (Enum.TryParse(learningUnitQuery.TaskType, out WritingTaskType taskType))
			{
				queryWithSteps = from l in queryWithSteps
								 join writing in _context.Writings.AsNoTracking()
								 on l.learningUnit.Id equals writing.LearningUnitId
								 where writing.TaskType == taskType
								 select new
								 {
									 learningUnit = l.learningUnit
								 };
			}
		}

		var entities = await queryWithSteps
			.GroupBy(x => x.learningUnit.Id)
			.OrderByDescending(g => g.First().learningUnit.CreatedAt)
			.Select(group => new LearningUnit
			{
				Id = group.Key,
				Title = group.First().learningUnit.Title,
				Level = group.First().learningUnit.Level.ToString(),
				SkillType = group.First().learningUnit.SkillType.ToString(),
				CreateAt = group.First().learningUnit.CreatedAt
			})
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.ToListAsync();

		var totalCount = 0;
		if (queryInfo.NeedTotalCount)
		{
			totalCount = await queryWithSteps.CountAsync();
		}

		return new QueryResult<LearningUnit>
		{
			Data = entities,
			TotalCount = totalCount
		};
	}


	public async Task<LearningUnit> GetLearningUnitByIdAsync(Guid id)
	{
		return await (from learningUnit in _context.LearningUnits.AsNoTracking()
					  where learningUnit.Id == id
					  select new LearningUnit
					  {
						  Id = learningUnit.Id,
						  Title = learningUnit.Title,
						  Level = learningUnit.Level.ToString(),
						  SkillType = learningUnit.SkillType.ToString()
					  }).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(LearningUnitEntity), id));
	}
}
