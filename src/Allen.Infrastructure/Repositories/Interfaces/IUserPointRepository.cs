namespace Allen.Infrastructure;

public interface IUserPointRepository
{
    Task<UserPoint?> GetUserPointByUserIdAsync(Guid userId);
    Task<QueryResult<UserPoint>> GetAllUserPointsAsync(QueryInfo queryInfo);
}
