namespace Allen.Application;

[RegisterService(typeof(IDashboardsServices))]
public class DashboardsServices(IDashboardsRepository _repository) : IDashboardsServices
{
	public async Task<GoalModel?> GetCurrentScoresAsync(Guid userId)
		=> await _repository.GetCurrentScoresAsync(userId);
	public async Task<QueryResult<ActivityModel>> GetActivityAsync(Guid userId, QueryInfo queryInfo)
		=> await _repository.GetActivityAsync(userId, queryInfo);
	public async Task<List<SummaryModel?>> GetWeeklySummaryAsync(Guid userId)
		=> await _repository.GetWeeklySummaryAsync(userId);
	public async Task<QueryResult<HistoryModel>> GetSubmissionHistoryAsync(Guid userId, QueryInfo queryInfo, SkillIeltsType skillIeltsType)
		=> await _repository.GetSubmissionHistoryAsync(userId, queryInfo, skillIeltsType);

	//#region mock data

	//public async Task<QueryResult<ActivityModel>> GetActivityAsync(Guid userId, QueryInfo queryInfo)
	//{
	//	await Task.Delay(100);

	//	var mockData = new List<ActivityModel>
	//	{
	//		new() { Date = DateTime.UtcNow.AddDays(-6), CompletedTasks = 2 },
	//		new() { Date = DateTime.UtcNow.AddDays(-5), CompletedTasks = 1 },
	//		new() { Date = DateTime.UtcNow.AddDays(-4), CompletedTasks = 3 },
	//		new() { Date = DateTime.UtcNow.AddDays(-3), CompletedTasks = 2 },
	//		new() { Date = DateTime.UtcNow.AddDays(-2), CompletedTasks = 1 },
	//		new() { Date = DateTime.UtcNow.AddDays(-1), CompletedTasks = 4 },
	//		new() { Date = DateTime.UtcNow, CompletedTasks = 3 }
	//	};

	//	return new QueryResult<ActivityModel>
	//	{
	//		Data = mockData,
	//		TotalCount = mockData.Count
	//	};
	//}

	//public async Task<GoalModel?> GetCurrentScoresAsync(Guid userId)
	//{
	//	// Giả lập dữ liệu band điểm hiện tại
	//	await Task.Delay(100); // mô phỏng async I/O
	//	return new GoalModel
	//	{
	//		UserId = userId,
	//		Overall = 7.0m,
	//		Reading = 7.5m,
	//		Listening = 7.0m,
	//		Writing = 6.5m,
	//		Speaking = 7.0m
	//	};
	//}

	//public async Task<QueryResult<HistoryModel>> GetSubmissionHistoryAsync(Guid userId, QueryInfo queryInfo, SkillIeltsType skillIeltsType)
	//{
	//	await Task.Delay(100); // giả lập delay async

	//	var history = new List<HistoryModel>
	//{
	//	new()
	//	{
	//		Id = Guid.NewGuid(),
	//		Title = "Listening Practice Test 1",
	//		SubmitAt = DateTime.UtcNow.AddDays(-7),
	//		TimeToDo = new DateTime(1, 1, 1, 0, 32, 15), // 32 phút 15 giây
 //           CorrectAnswers = 32,
	//		UnCorrectAnswers = 8,
	//		TotalQuestions = 40,
	//		PercentageCorrect = Math.Round(32.0 / 40 * 100, 1),
	//		Type = "Listening",
	//		Score = 7.0
	//	},
	//	new()
	//	{
	//		Id = Guid.NewGuid(),
	//		Title = "Reading Practice Test 2",
	//		SubmitAt = DateTime.UtcNow.AddDays(-5),
	//		TimeToDo = new DateTime(1, 1, 1, 1, 2, 0), // 1h02p
 //           CorrectAnswers = 35,
	//		UnCorrectAnswers = 5,
	//		TotalQuestions = 40,
	//		PercentageCorrect = Math.Round(35.0 / 40 * 100, 1),
	//		Type = "Reading",
	//		Score = 7.5
	//	},
	//	new()
	//	{
	//		Id = Guid.NewGuid(),
	//		Title = "Writing Task 1 - Graph",
	//		SubmitAt = DateTime.UtcNow.AddDays(-3),
	//		TimeToDo = new DateTime(1, 1, 1, 0, 45, 0),
	//		CorrectAnswers = 0, // Writing không có đúng/sai
 //           UnCorrectAnswers = 0,
	//		TotalQuestions = 0,
	//		PercentageCorrect = 0,
	//		Type = "Writing",
	//		Score = 6.5
	//	},
	//	new()
	//	{
	//		Id = Guid.NewGuid(),
	//		Title = "Speaking Practice Test",
	//		SubmitAt = DateTime.UtcNow.AddDays(-1),
	//		TimeToDo = new DateTime(1, 1, 1, 0, 14, 0),
	//		CorrectAnswers = 0,
	//		UnCorrectAnswers = 0,
	//		TotalQuestions = 0,
	//		PercentageCorrect = 0,
	//		Type = "Speaking",
	//		Score = 7.0
	//	},
	//	new()
	//	{
	//		Id = Guid.NewGuid(),
	//		Title = "Listening Practice Test 2",
	//		SubmitAt = DateTime.UtcNow.AddDays(-1),
	//		TimeToDo = new DateTime(1, 1, 1, 0, 30, 45),
	//		CorrectAnswers = 34,
	//		UnCorrectAnswers = 6,
	//		TotalQuestions = 40,
	//		PercentageCorrect = Math.Round(34.0 / 40 * 100, 1),
	//		Type = "Listening",
	//		Score = 7.5
	//	}
	//};

	//	return new QueryResult<HistoryModel>
	//	{
	//		Data = history.OrderByDescending(h => h.SubmitAt),
	//		TotalCount = history.Count
	//	};
	//}

	//public async Task<List<SummaryModel?>> GetWeeklySummaryAsync(Guid userId)
	//{
	//	await Task.Delay(100); // Giả lập async
	//	var random = new Random();

	//	// Xác định ngày đầu tuần (thứ 2) và cuối tuần (chủ nhật)
	//	var today = DateTime.Today;
	//	int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
	//	var startOfWeek = today.AddDays(-diff);

	//	var summaries = new List<SummaryModel?>();

	//	// Tạo dữ liệu 7 ngày trong tuần (Thứ 2 → Chủ nhật)
	//	for (int i = 0; i < 7; i++)
	//	{
	//		var date = startOfWeek.AddDays(i);

	//		summaries.Add(new SummaryModel
	//		{
	//			Date = date,
	//			ReadingCount = date.DayOfWeek == DayOfWeek.Monday ? 1 : 0, // ví dụ: chỉ có Reading vào thứ 2
	//			ListeningCount = random.Next(1,99),
	//			WritingCount = random.Next(1, 99),
	//			SpeakingCount = random.Next(1, 99),
	//			TotalMinutes = random.Next(1, 999)
	//		});
	//	}

	//	return summaries;
	//}
	//#endregion
	// revenue report
	public async Task<RevenueSummaryModel> GetRevenueAsync(RevenueSummaryQuery rsQuery)
    {
        return await _repository.GetRevenueSummaryAsync(rsQuery);
    }
}
