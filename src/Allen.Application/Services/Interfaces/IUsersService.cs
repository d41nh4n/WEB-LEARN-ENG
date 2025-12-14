namespace Allen.Application;

public interface IUsersService : IServiceBase<User>
{
    Task<QueryResult<User>> GetUsersWithPagingAsync(QueryInfo queryInfo);
    Task<UserInfoModel> GetByIdAsync(Guid id);
	Task<List<RecommendedUnitsResponse>> RecommendAsync(UserBandModel band, Guid userId);
	Task<User> LoginAsync(LoginModel model);
    Task<User> GetByRefreshTokenAsync(string refreshToken);
    Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id);
    Task<OperationResult> CreateAsync(RegisterModel model);
    Task<OperationResult> BlockUserAsync(BlockUserModel model);
    Task<OperationResult> BanOrUnbanAsync(BanUserModel model);
    Task<OperationResult> UnblockUserAsync(BlockUserModel model);
    Task<OperationResult> UpdateAsync(Guid id, UpdateUserModel model);
    Task UpdateRefreshTokenAsync(Guid id, string refreshToken);
}
