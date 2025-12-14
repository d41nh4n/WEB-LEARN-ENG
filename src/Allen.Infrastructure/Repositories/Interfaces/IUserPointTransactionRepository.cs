namespace Allen.Infrastructure;

public interface IUserPointTransactionRepository
{
    Task<QueryResult<UserPointTransaction>> GetAllTransactionsAsync(QueryInfo queryInfo);
    Task<QueryResult<UserPointTransaction>> GetTransactionsByUserIdAsync(Guid userId, QueryInfo queryInfo);
}