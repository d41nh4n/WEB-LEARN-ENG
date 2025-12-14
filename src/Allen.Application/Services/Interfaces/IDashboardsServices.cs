namespace Allen.Application;

public interface IDashboardsServices
{
	Task<GoalModel?> GetCurrentScoresAsync(Guid userId);
	Task<QueryResult<ActivityModel>> GetActivityAsync(Guid userId, QueryInfo queryInfo);
	Task<List<SummaryModel?>> GetWeeklySummaryAsync(Guid userId);
	Task<QueryResult<HistoryModel>> GetSubmissionHistoryAsync(Guid userId, QueryInfo queryInfo, SkillIeltsType skillIeltsType);

    Task<RevenueSummaryModel> GetRevenueAsync(RevenueSummaryQuery rsQuery);
}