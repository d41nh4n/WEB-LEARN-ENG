using Net.payOS.Types;

namespace Allen.Application;

public interface IPayOSService
{
    Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentData data);
    WebhookData VerifyWebhook(WebhookType body);
    Task<PaymentLinkInformation> GetPaymentLinkInfoAsync(long orderCode);
    Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderCode, string? cancelReason);
}
