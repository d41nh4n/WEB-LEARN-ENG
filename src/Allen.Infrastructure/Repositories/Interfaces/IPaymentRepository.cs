namespace Allen.Infrastructure;

public interface IPaymentRepository : IRepositoryBase<PaymentEntity>
{
    Task<PaymentEntity?> GetByOrderCodeAsync(long orderCode);

    Task<QueryResult<Payment>> GetAllPaidPaymentsAsync(QueryInfo queryInfo);
    Task<QueryResult<Payment>> GetPaidPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo);
}