using Net.payOS.Types;

namespace Allen.Application;

public interface IPaymentService
{
    Task<QueryResult<Payment>> GetAllPaidPaymentsAsync(QueryInfo queryInfo);
    Task<QueryResult<Payment>> GetPaidPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo);

    Task<OperationResult> BuyPackageAsync(Guid userId, Guid packageId);
    Task HandleWebhookAsync(WebhookType payload);
}