namespace Allen.API.Controllers;

[Route("dashboards")]
[Authorize]
public class DashboardsController(IDashboardsServices _services) : BaseApiController
{
	[HttpGet("current-score")]
	public async Task<GoalModel?> GetCurrentScores()
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _services.GetCurrentScoresAsync(userId);
	}

	[HttpGet("activity")]
	public async Task<QueryResult<ActivityModel>> GetActivity([FromQuery] QueryInfo queryInfo)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _services.GetActivityAsync(userId, queryInfo);
	}

	[HttpGet("summary")]
	public async Task<List<SummaryModel?>> GetWeeklySummary()
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _services.GetWeeklySummaryAsync(userId);
	}

	[HttpGet("history")]
	public async Task<QueryResult<HistoryModel>> GetSubmissionHistory([FromQuery] QueryInfo queryInfo, [FromQuery] SkillIeltsType skillIeltsType)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _services.GetSubmissionHistoryAsync(userId, queryInfo, skillIeltsType);
	}

    // revenue report
    //[Authorize(Roles = "Admin")]
	[HttpGet("revenueSumary")]
    [ValidateModel]
    public async Task<RevenueSummaryModel> GetRevenueSummary([FromQuery] RevenueSummaryQuery rsQuery)
    {
        return await _services.GetRevenueAsync(rsQuery);
    }
}
