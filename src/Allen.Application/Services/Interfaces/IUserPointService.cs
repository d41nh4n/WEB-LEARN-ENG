namespace Allen.Application;

public interface IUserPointService
{
    Task<QueryResult<UserPoint>> GetAllUserPointsAsync(QueryInfo queryInfo);
    Task<OperationResult> GetUserPointsByUserIdAsync(Guid userId);

    Task<OperationResult> AddPointsAsync(Guid userId, PackageModel package, Guid paymentId);
    Task<OperationResult> UsePointsAsync(Guid userId, int pointsToUse);

    Task<OperationResult> AddPointsInternalAsync(AddPointsModel model);
    Task<bool> UsePointsInternalAsync(Guid userId, int pointsToUse);
}