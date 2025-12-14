namespace Allen.Application;

public interface IUserPointTransactionService
{
    Task<QueryResult<UserPointTransaction>> GetAllTransactionsAsync(QueryInfo queryInfo);
    Task<QueryResult<UserPointTransaction>> GetTransactionsByUserIdAsync(Guid userId, QueryInfo queryInfo);
}
