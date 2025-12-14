namespace Allen.Application;

[RegisterService(typeof(IUserPointTransactionService))]
public class UserPointTransactionService(IUserPointTransactionRepository _repository, IUnitOfWork _unitOfWork) : IUserPointTransactionService
{
    public async Task<QueryResult<UserPointTransaction>> GetAllTransactionsAsync(QueryInfo queryInfo)
        => await _repository.GetAllTransactionsAsync(queryInfo);

    public async Task<QueryResult<UserPointTransaction>> GetTransactionsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        return await _repository.GetTransactionsByUserIdAsync(userId, queryInfo);
    }
}
