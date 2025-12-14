using Net.payOS;
using Net.payOS.Types;

namespace Allen.Application;

[RegisterService(typeof(IPayOSService))]
public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly string _cancelUrl;
    private readonly string _returnUrl;

    public PayOSService(IConfiguration config)
    {
        var section = config.GetSection("PayOS");

        string clientId = section["ClientId"]
                     ?? throw new ArgumentNullException(nameof(clientId), "PayOS ClientId not configured");
        string apiKey = section["ApiKey"]
                     ?? throw new ArgumentNullException(nameof(apiKey), "PayOS ApiKey not configured");
        string checksumKey = section["ChecksumKey"]
                     ?? throw new ArgumentNullException(nameof(checksumKey), "PayOS ChecksumKey not configured");
        _returnUrl = section["ReturnUrl"]
                     ?? throw new ArgumentNullException(nameof(_returnUrl), "PayOS ReturnUrl not configured");
        _cancelUrl = section["CancelUrl"]
                     ?? throw new ArgumentNullException(nameof(_cancelUrl), "PayOS CancelUrl not configured");

        _payOS = new PayOS(clientId, apiKey, checksumKey);
    }

    public async Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentData data)
    {
        var finalData = new PaymentData(
            data.orderCode,
            data.amount,
            data.description,
            data.items,
            _cancelUrl,
            _returnUrl
        );
        return await _payOS.createPaymentLink(finalData);
    }

    public async Task<PaymentLinkInformation> GetPaymentLinkInfoAsync(long orderCode)
    {
        return await _payOS.getPaymentLinkInformation(orderCode);
    }

    public async Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderCode, string? cancelReason)
    {
        return string.IsNullOrWhiteSpace(cancelReason)
            ? await _payOS.cancelPaymentLink(orderCode).ConfigureAwait(false)
            : await _payOS.cancelPaymentLink(orderCode, cancelReason).ConfigureAwait(false);
    }

    public WebhookData VerifyWebhook(WebhookType body)
    {
        return _payOS.verifyPaymentWebhookData(body);
    }
}
