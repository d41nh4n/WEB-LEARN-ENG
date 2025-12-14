namespace Allen.API.Controllers;

[Route("users")]
[Authorize]
public class UsersController(IUsersService _usersService, 
    IUserPointService _userPointService,
    IUserPointTransactionService _userPointTransactionService) : BaseApiController
{
    #region User
    [HttpGet("paging")]
    public async Task<QueryResult<User>> GetUsersWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _usersService.GetUsersWithPagingAsync(queryInfo);
    }
    [HttpGet("{id}")]
    public async Task<UserInfoModel> GetUserById(Guid id)
    {
        return await _usersService.GetByIdAsync(id);
    }
	[HttpPost("recommend")]
	public async Task<List<RecommendedUnitsResponse>> Recommend([FromBody] UserBandModel dto)
	{
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		return await _usersService.RecommendAsync(dto, userId);
	}

	[HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update(Guid id, [FromForm] UpdateUserModel model)
    {
        return await _usersService.UpdateAsync(id, model);
    }
    [HttpPost("user-block")]
    public async Task<OperationResult> BlockUser([FromBody] BlockUserModel model)
    {
        model.BlockedByUserId = HttpContextHelper.GetCurrentUserId(HttpContext);
        return await _usersService.BlockUserAsync(model);
    }

    [HttpDelete("{blockedUserId}/userblock")]
    public async Task<OperationResult> UnblockUser([FromRoute] Guid blockedUserId)
    {
        var blockedByUserId = HttpContextHelper.GetCurrentUserId(HttpContext);
        var model = new BlockUserModel
        {
            BlockedByUserId = blockedByUserId,
            BlockedUserId = blockedUserId
        };
        return await _usersService.UnblockUserAsync(model);
    }
    [HttpPost("ban")]
    [ValidateModel]
    public Task<OperationResult> BanOrUnban([FromBody] BanUserModel model)
    {
        return _usersService.BanOrUnbanAsync(model);
    }
    #endregion

    #region User Points
    [HttpGet("points/all")]
    [Authorize (Roles = "Admin")]
    public async Task<QueryResult<UserPoint>> GetAll([FromQuery] QueryInfo queryInfo)
        => await _userPointService.GetAllUserPointsAsync(queryInfo);

    [HttpGet("points/byUser/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<OperationResult> GetPointsByUserId(Guid userId)
    {
        return await _userPointService.GetUserPointsByUserIdAsync(userId);
    }

    [HttpGet("points/byUser")]
    public async Task<OperationResult> GetPointsByUserId()
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _userPointService.GetUserPointsByUserIdAsync(userId);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("addPoints")]
    [ValidateModel]
    public async Task<IActionResult> AddPoints([FromBody] AddPointsModel model)
    {
        var result = await _userPointService.AddPointsInternalAsync(model);
        return Ok(result);
    }

    [HttpPost("usePoints")]
    public async Task<IActionResult> UsePoints([FromBody] UsePointsModel model)
    {
        var userId = HttpContext.GetCurrentUserId();
        var result = await _userPointService.UsePointsAsync(userId, model.PointsToUse);
        return Ok(result);
    }
    #endregion

    #region User Point Transactions
    [HttpGet("pointTransactions/all")]
    [Authorize(Roles = "Admin")]
    public async Task<QueryResult<UserPointTransaction>> GetAllTransactionsAsync([FromQuery] QueryInfo queryInfo)
    {
        return await _userPointTransactionService.GetAllTransactionsAsync(queryInfo);
    }

    [HttpGet("pointTransactions/byUser/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<QueryResult<UserPointTransaction>> GetTransactionsByUserIdAsync(Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        return await _userPointTransactionService.GetTransactionsByUserIdAsync(userId, queryInfo);
    }

    [HttpGet("pointTransactions/byUser")]
    public async Task<QueryResult<UserPointTransaction>> GetTransactionsByUserIdAsync([FromQuery] QueryInfo queryInfo)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _userPointTransactionService.GetTransactionsByUserIdAsync(userId, queryInfo);
    }
    #endregion
}
