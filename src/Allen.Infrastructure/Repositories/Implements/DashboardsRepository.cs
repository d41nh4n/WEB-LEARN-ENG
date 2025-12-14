namespace Allen.Infrastructure;

[RegisterService(typeof(IDashboardsRepository))]
public class DashboardsRepository(SqlApplicationDbContext context) : IDashboardsRepository
{
	private readonly SqlApplicationDbContext _context = context;

	public async Task<GoalModel?> GetCurrentScoresAsync(Guid userId)
	{
		var query = from uta in _context.UserTestAttempts.AsNoTracking()
					where uta.UserId == userId
					join learningUnit in _context.LearningUnits.AsNoTracking()
					on uta.LearningUnitId equals learningUnit.Id
					select new { uta, learningUnit };

		// Tính trung bình cho từng kỹ năng, tránh lỗi nếu không có bài nào
		var reading = await query
			.Where(x => x.learningUnit.LearningUnitType == LearningUnitType.Ielts &&
						x.learningUnit.SkillType == SkillType.Reading)
			.AverageAsync(x => (double?)x.uta.OverallBand) ?? 0;

		var speaking = await query
			.Where(x => x.learningUnit.LearningUnitType == LearningUnitType.Ielts &&
						x.learningUnit.SkillType == SkillType.Speaking)
			.AverageAsync(x => (double?)x.uta.OverallBand) ?? 0;

		var listening = await query
			.Where(x => x.learningUnit.LearningUnitType == LearningUnitType.Ielts &&
						x.learningUnit.SkillType == SkillType.Listening)
			.AverageAsync(x => (double?)x.uta.OverallBand) ?? 0;

		var writing = await query
			.Where(x => x.learningUnit.LearningUnitType == LearningUnitType.Ielts &&
						x.learningUnit.SkillType == SkillType.Writing)
			.AverageAsync(x => (double?)x.uta.OverallBand) ?? 0;

		var overallRaw = (reading + speaking + listening + writing) / 4;

		// Làm tròn theo quy tắc IELTS
		double RoundIelts(double score)
		{
			var fraction = score - Math.Floor(score);
			if (fraction < 0.25) return Math.Floor(score);
			if (fraction < 0.75) return Math.Floor(score) + 0.5;
			return Math.Ceiling(score);
		}

		var result = new GoalModel
		{
			UserId = userId,
			Reading = (decimal)RoundIelts(reading),
			Speaking = (decimal)RoundIelts(speaking),
			Listening = (decimal)RoundIelts(listening),
			Writing = (decimal)RoundIelts(writing),
			Overall = (decimal)RoundIelts(overallRaw)
		};

		return result;
	}

	public async Task<QueryResult<ActivityModel>> GetActivityAsync(Guid userId, QueryInfo queryInfo)
	{
		var today = DateTime.UtcNow.Date;

		// Mỗi trang = 2 tháng
		int monthsPerPage = 2;

		// Trang bắt đầu từ 0 hoặc 1, nên tính offset tháng
		// skip = 0 → 2 tháng gần nhất
		// skip = 1 → 2 tháng kế trước đó
		int pageIndex = queryInfo.Skip; // Skip ở đây coi như "index" của trang

		var endDate = today.AddMonths(-monthsPerPage * pageIndex);
		var startDate = endDate.AddMonths(-monthsPerPage).AddDays(1 - endDate.Day); // bắt đầu từ ngày đầu tháng của khoảng đó

		// Base query
		var query = _context.UserTestAttempts
			.AsNoTracking()
			.Where(x => x.UserId == userId &&
						x.CreatedAt.HasValue &&
						x.CreatedAt.Value.Date >= startDate &&
						x.CreatedAt.Value.Date <= endDate);

		// Gom nhóm theo ngày
		var grouped = query
			.GroupBy(x => x.CreatedAt!.Value.Date)
			.Select(g => new ActivityModel
			{
				Date = g.Key,
				CompletedTasks = g.Count()
			});

		// Sắp xếp
		grouped = queryInfo.OrderType == OrderType.Ascending
			? grouped.OrderBy(x => x.Date)
			: grouped.OrderByDescending(x => x.Date);

		var data = await grouped.Take(queryInfo.Top).ToListAsync();

		return new QueryResult<ActivityModel>
		{
			Data = data,
			TotalCount = data.Count
		};
	}

	public async Task<List<SummaryModel?>> GetWeeklySummaryAsync(Guid userId)
	{
		var today = DateTime.UtcNow.Date;
		int dayOfWeek = (int)today.DayOfWeek;
		var startOfWeek = today.AddDays(dayOfWeek == 0 ? -6 : -dayOfWeek + 1);
		var endOfWeek = startOfWeek.AddDays(6);

		var query = from uta in _context.UserTestAttempts.AsNoTracking()
					where uta.UserId == userId
					join learningUnit in _context.LearningUnits.AsNoTracking()
					on uta.LearningUnitId equals learningUnit.Id
					where learningUnit.LearningUnitType == LearningUnitType.Ielts
					select new { uta, learningUnit };

		var grouped = await query
			.Where(x => x.uta.CreatedAt!.Value.Date >= startOfWeek &&
						x.uta.CreatedAt!.Value.Date <= endOfWeek)
			.GroupBy(x => x.uta.CreatedAt!.Value.Date)
			.Select(g => new SummaryModel
			{
				Date = g.Key,
				ReadingCount = g.Count(x => x.learningUnit.SkillType == SkillType.Reading),
				ListeningCount = g.Count(x => x.learningUnit.SkillType == SkillType.Listening),
				WritingCount = g.Count(x => x.learningUnit.SkillType == SkillType.Writing),
				SpeakingCount = g.Count(x => x.learningUnit.SkillType == SkillType.Speaking),
				TotalMinutes = g.Sum(x => x.uta.TimeSpent)
			})
			.OrderBy(x => x.Date)
			.ToListAsync();

		return grouped!;
	}


	public async Task<QueryResult<HistoryModel>> GetSubmissionHistoryAsync(
	Guid userId,
	QueryInfo queryInfo,
	SkillIeltsType skillIeltsType)
	{
		var baseQuery = from uta in _context.UserTestAttempts.AsNoTracking()
						join learningUnit in _context.LearningUnits.AsNoTracking()
							on uta.LearningUnitId equals learningUnit.Id
						where uta.UserId == userId
							  && learningUnit.SkillType.HasValue
							  && (SkillIeltsType)learningUnit.SkillType.Value == skillIeltsType
							  && learningUnit.LearningUnitType == LearningUnitType.Ielts
						select new { uta, learningUnit };	

		var testList = await baseQuery
			.OrderByDescending(x => x.uta.CreatedAt)
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.ToListAsync();

		var totalCount = queryInfo.NeedTotalCount
			? await baseQuery.CountAsync()
			: 0;

		if (!testList.Any())
		{
			return new QueryResult<HistoryModel>
			{
				Data = new List<HistoryModel>(),
				TotalCount = totalCount
			};
		}

		Dictionary<Guid, List<UserAnswerEntity>> userAnswersByAttempt = new();

		if (skillIeltsType == SkillIeltsType.Reading || skillIeltsType == SkillIeltsType.Listening)
		{
			var attemptIds = testList.Select(x => x.uta.Id).ToList();

			userAnswersByAttempt = await _context.UserAnswers
				.AsNoTracking()
				.Where(a => attemptIds.Contains(a.AttemptId))
				.GroupBy(a => a.AttemptId)
				.ToDictionaryAsync(g => g.Key, g => g.ToList());
		}

		var data = testList.Select(x =>
		{
			var answers = userAnswersByAttempt.ContainsKey(x.uta.Id)
				? userAnswersByAttempt[x.uta.Id]
				: new List<UserAnswerEntity>();

			var total = answers.Count;
			var correct = answers.Count(a => a.IsCorrect == true);
			var incorrect = answers.Count(a => a.IsCorrect == false);
			var percent = total > 0 ? (double)correct / total * 100 : 0;

			return new HistoryModel
			{
				Id = x.uta.Id,
				LearningUnitId = x.learningUnit.Id,
				Title = x.learningUnit.Title,
				SubmitAt = x.uta.CreatedAt ?? DateTime.MinValue,
				TimeToDo = x.uta.TimeSpent,
				CorrectAnswers = correct,
				UnCorrectAnswers = incorrect,
				TotalQuestions = total,
				PercentageCorrect = percent,
				Type = x.learningUnit.SkillType.ToString() ?? string.Empty,
				Score = (double)x.uta.OverallBand
			};
		}).ToList();

		return new QueryResult<HistoryModel>
		{
			Data = data,
			TotalCount = totalCount
		};
	}

	#region revenue reports
	public async Task<RevenueSummaryModel> GetRevenueSummaryAsync(RevenueSummaryQuery rsQuery)
	{
		var fromDate = rsQuery.FromDate!.Value.Date;
		var toDate = rsQuery.ToDate!.Value.Date.AddDays(1).AddTicks(-1);

		var query = _context.Payments
			.AsNoTracking()
			.Where(p => p.Status == "PAID" && p.CreatedAt.HasValue &&
						p.CreatedAt.Value >= fromDate && p.CreatedAt.Value <= toDate);

		var groupMode = DetectGroupMode(fromDate, toDate);
		var items = await GetRevenueItemsAsync(query, fromDate, toDate, groupMode);

		var totalRevenue = items.Sum(x => x.TotalAmount);
		var totalOrders = items.Sum(x => x.TotalOrders);
		var averageRevenue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

		return new RevenueSummaryModel
		{
			FromDate = fromDate,
			ToDate = toDate,
			GroupBy = groupMode,
			TotalRevenue = totalRevenue,
			TotalOrders = totalOrders,
			AverageRevenue = averageRevenue,
			Items = items
		};
	}

	private static string DetectGroupMode(DateTime fromDate, DateTime toDate)
	{
		fromDate = fromDate.Date;
		toDate = toDate.Date;

		if (fromDate.Month == 1 && fromDate.Day == 1 &&
			toDate.Month == 12 && toDate.Day == 31 &&
			fromDate.Year == toDate.Year)
			return "year";

		var quarters = new[]
		{
			(Start: new DateTime(fromDate.Year, 1, 1), End: new DateTime(fromDate.Year, 3, 31)),
			(Start: new DateTime(fromDate.Year, 4, 1), End: new DateTime(fromDate.Year, 6, 30)),
			(Start: new DateTime(fromDate.Year, 7, 1), End: new DateTime(fromDate.Year, 9, 30)),
			(Start: new DateTime(fromDate.Year, 10, 1), End: new DateTime(fromDate.Year, 12, 31))
		};
		if (quarters.Any(q => q.Start == fromDate && q.End == toDate))
			return "quarter";

		if (fromDate.Year == toDate.Year && fromDate.Month == toDate.Month &&
			fromDate.Day == 1 && toDate.Day >= DateTime.DaysInMonth(toDate.Year, toDate.Month))
			return "month";

		var startOfWeek = fromDate.AddDays(-(int)fromDate.DayOfWeek + (fromDate.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
		var endOfWeek = startOfWeek.AddDays(6);
		if (fromDate == startOfWeek && toDate == endOfWeek)
			return "week";

		return "day";
	}
	private async Task<List<RevenueItem>> GetRevenueItemsAsync(IQueryable<PaymentEntity> query, DateTime fromDate, DateTime toDate, string groupMode)
	{
		return groupMode switch
		{
			"year" => await GetYearlyRevenue(query, fromDate),
			"quarter" => await GetQuarterlyRevenue(query, fromDate),
			"month" => await GetMonthlyRevenue(query),
			"week" => await GetWeeklyRevenueAsync(query),
			_ => await GetDailyRevenueAsync(query)
		};
	}
	private async Task<List<RevenueItem>> GetYearlyRevenue(IQueryable<PaymentEntity> query, DateTime fromDate)
	{
		var year = fromDate.Year;

		var data = await query
			.Where(p => p.CreatedAt!.Value.Year == year)
			.GroupBy(p => new { p.CreatedAt!.Value.Year, p.CreatedAt!.Value.Month })
			.Select(g => new
			{
				g.Key.Year,
				g.Key.Month,
				TotalAmount = g.Sum(x => x.Amount),
				TotalOrders = g.Count(),
				AverageAmount = g.Average(x => x.Amount)
			})
			.OrderBy(x => x.Year).ThenBy(x => x.Month)
			.ToListAsync();

		var result = data.Select(x => new RevenueItem
		{
			Period = $"{x.Year}-{x.Month:D2}",
			TotalAmount = x.TotalAmount,
			TotalOrders = x.TotalOrders,
			AverageAmount = x.AverageAmount
		}).ToList();

		for (int m = 1; m <= 12; m++)
		{
			var period = $"{year}-{m:D2}";
			if (!result.Any(x => x.Period == period))
				result.Add(new RevenueItem { Period = period });
		}

		return result.OrderBy(x => x.Period).ToList();
	}
	private async Task<List<RevenueItem>> GetQuarterlyRevenue(IQueryable<PaymentEntity> query, DateTime fromDate)
	{
		int quarter = (fromDate.Month - 1) / 3 + 1;
		int startMonth = (quarter - 1) * 3 + 1;
		int endMonth = startMonth + 2;
		int year = fromDate.Year;

		var data = await query
			.Where(p => p.CreatedAt!.Value.Year == year &&
					p.CreatedAt!.Value.Month >= startMonth &&
					p.CreatedAt!.Value.Month <= endMonth)
			.GroupBy(p => new { p.CreatedAt!.Value.Year, p.CreatedAt!.Value.Month })
			.Select(g => new
			{
				g.Key.Year,
				g.Key.Month,
				TotalAmount = g.Sum(x => x.Amount),
				TotalOrders = g.Count(),
				AverageAmount = g.Average(x => x.Amount)
			})
			.OrderBy(x => x.Year).ThenBy(x => x.Month)
			.ToListAsync();

		var result = data.Select(x => new RevenueItem
		{
			Period = $"{x.Year}-{x.Month:D2}",
			TotalAmount = x.TotalAmount,
			TotalOrders = x.TotalOrders,
			AverageAmount = x.AverageAmount
		}).ToList();

		for (int m = startMonth; m < startMonth + 3; m++)
		{
			var period = $"{year}-{m:D2}";
			if (!result.Any(x => x.Period == period))
				result.Add(new RevenueItem { Period = period });
		}

		return result.OrderBy(x => x.Period).ToList();
	}
	private async Task<List<RevenueItem>> GetMonthlyRevenue(IQueryable<PaymentEntity> query)
	{
		var data = await query
			.GroupBy(p => new { p.CreatedAt!.Value.Year, p.CreatedAt!.Value.Month })
			.Select(g => new
			{
				g.Key.Year,
				g.Key.Month,
				TotalAmount = g.Sum(x => x.Amount),
				TotalOrders = g.Count(),
				AverageAmount = g.Average(x => x.Amount)
			})
			.OrderBy(x => x.Year).ThenBy(x => x.Month)
			.ToListAsync();

		return data.Select(x => new RevenueItem
		{
			Period = $"{x.Year}-{x.Month:D2}",
			TotalAmount = x.TotalAmount,
			TotalOrders = x.TotalOrders,
			AverageAmount = x.AverageAmount
		}).ToList();
	}
	private async Task<List<RevenueItem>> GetWeeklyRevenueAsync(IQueryable<PaymentEntity> query)
	{
		var data = await query
			.Select(p => new { p.Amount, p.CreatedAt })
			.ToListAsync();

		return data
			.GroupBy(p => new
			{
				Year = p.CreatedAt!.Value.Year,
				Week = System.Globalization.ISOWeek.GetWeekOfYear(p.CreatedAt!.Value)
			})
			.Select(g => new RevenueItem
			{
				Period = $"{g.Key.Year}-W{g.Key.Week:D2}",
				TotalAmount = g.Sum(x => x.Amount),
				TotalOrders = g.Count(),
				AverageAmount = g.Average(x => x.Amount)
			})
			.OrderBy(x => x.Period)
			.ToList();
	}
	private async Task<List<RevenueItem>> GetDailyRevenueAsync(IQueryable<PaymentEntity> query)
	{
		var data = await query
			.GroupBy(p => p.CreatedAt!.Value.Date)
			.Select(g => new
			{
				Date = g.Key,
				TotalAmount = g.Sum(x => x.Amount),
				TotalOrders = g.Count(),
				AverageAmount = g.Average(x => x.Amount)
			})
			.OrderBy(x => x.Date)
			.ToListAsync();

		return data.Select(x => new RevenueItem
		{
			Period = x.Date.ToString("yyyy-MM-dd"),
			TotalAmount = x.TotalAmount,
			TotalOrders = x.TotalOrders,
			AverageAmount = x.AverageAmount
		}).ToList();
	}
	private RevenueItem MapToRevenueItem(dynamic g)
	{
		return new RevenueItem
		{
			Period = $"{g.Year}-{g.Month:D2}",
			TotalAmount = g.TotalAmount,
			TotalOrders = g.TotalOrders,
			AverageAmount = g.AverageAmount
		};
	}
	#endregion
}