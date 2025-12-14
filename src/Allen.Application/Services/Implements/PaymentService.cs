using Net.payOS.Types;

namespace Allen.Application;

[RegisterService(typeof(IPaymentService))]
public class PaymentService(
    IPayOSService _payOS,
    IUnitOfWork _unitOfWork,
    IUserPointService _userPoint,
    IPaymentRepository _repository
) : IPaymentService
{
    public async Task<QueryResult<Payment>> GetAllPaidPaymentsAsync(QueryInfo queryInfo)
    {
        return await _repository.GetAllPaidPaymentsAsync(queryInfo);
    }

    public async Task<QueryResult<Payment>> GetPaidPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        return await _repository.GetPaidPaymentsByUserIdAsync(userId, queryInfo);
    }

    public async Task<OperationResult> BuyPackageAsync(Guid userId, Guid packageId)
    {
        // Check user exist
        var user = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        if (user == null || user.IsDeleted == true)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(UserEntity), userId));

        // 1. Lấy thông tin gói
        var package = await _unitOfWork.Repository<PackageEntity>().GetByIdAsync(packageId);
        if (package == null || !package.IsActive)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(PackageEntity), packageId));

        // 2. Tạo PaymentData gửi sang PayOS
        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var data = new PaymentData(
            orderCode,
            (int)package.Price,
            $"Buy package {package.Name}",
            new List<ItemData> { new ItemData(package.Name ?? "Package", 1, (int)package.Price) },
            "",
            ""
        );

        var payOSResult = await _payOS.CreatePaymentLinkAsync(data);

        // 3. Lưu thông tin Payment vào DB
        var payment = new PaymentEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PackageId = package.Id,
            OrderCode = payOSResult.orderCode,
            Amount = payOSResult.amount,
            Description = payOSResult.description,
            Currency = payOSResult.currency,
            PaymentLinkId = payOSResult.paymentLinkId,
            Status = payOSResult.status,
            CheckoutUrl = payOSResult.checkoutUrl,
            AccountNumber = payOSResult.accountNumber,
            Bin = payOSResult.bin,
            QrCode = payOSResult.qrCode,
            ExpiredAt = payOSResult.expiredAt.HasValue
                ? DateTimeOffset.FromUnixTimeMilliseconds(payOSResult.expiredAt.Value).UtcDateTime
                : null
        };

        await _repository.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // 4. Trả về kết quả (OperationResult)
        return OperationResult.SuccessResult(ErrorMessageBase.Format(
            ErrorMessageBase.CreatedSuccess, nameof(PaymentEntity)), new { payment.CheckoutUrl });
    }

    public async Task HandleWebhookAsync(WebhookType payload)
    {
        var data = _payOS.VerifyWebhook(payload);
        var payment = await _repository.GetByOrderCodeAsync(data.orderCode);
        if (payment == null) return;

        payment.Status = data.code switch
        {
            "00" or "PAID" or "PAYMENT_SUCCESS" => "PAID",
            "09" or "CANCELLED" or "PAYMENT_CANCELLED" => "CANCELLED",
            "99" or "FAILED" or "PAYMENT_FAILED" => "FAILED",
            _ => "PENDING"
        };
        payment.LastModified = DateTime.UtcNow;

        _repository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        if (payment.Status == "PAID")
            await ProcessPaidPaymentAsync(payment);
    }

    private async Task ProcessPaidPaymentAsync(PaymentEntity payment)
    {
        if (!payment.PackageId.HasValue)
            return;

        var existed = await _unitOfWork.Repository<UserPointTransactionEntity>().CheckExistAsync(t => t.PaymentId == payment.Id);
        if (existed)
            return;

        var package = await _unitOfWork.Repository<PackageEntity>().GetByIdAsync(payment.PackageId.Value);
        if (package == null)
            return;

        var pkg = new PackageModel
        {
            Id = package.Id,
            Name = package.Name,
            Points = package.Points,
            Description = package.Description
        };

        try
        {
            var result = await _userPoint.AddPointsAsync(payment.UserId, pkg, payment.Id);
            //Console.WriteLine($"[Webhook] AddPointsAsync result: {result.Message}");
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"[Webhook] AddPointsAsync error: {ex.Message}\n{ex.StackTrace}");
            var combinedMessage = $"{ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(UserPointEntity))} Inner: {ex.Message}";
            throw new InternalServerException("InternalServer", combinedMessage);
        }
    }
}