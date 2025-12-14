namespace Allen.Infrastructure;

public interface IUsersRepository : IRepositoryBase<UserEntity>
{
	Task<QueryResult<User>> GetUsersWithPagingAsync(QueryInfo queryInfo);
	Task<UserInfoModel> GetUserByIdAsync(Guid id);
	Task<UserForTokenModel?> GetUserTokenDataAsync(Guid id);
	Task<User> GetByRefreshTokenAsync(string refreshToken);
	Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id);
	Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id);
	Task<List<LearningUnitEntity>> GetUnitsForSkillAsync(SkillType skill, LevelType targetLevel, Guid userId, int limit = 10);
}
