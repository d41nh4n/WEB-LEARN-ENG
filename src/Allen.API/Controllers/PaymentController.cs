using Net.payOS.Types;

namespace Allen.API.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController(IPaymentService _service) : ControllerBase
{
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<QueryResult<Payment>> GetAll([FromQuery] QueryInfo queryInfo)
        => await _service.GetAllPaidPaymentsAsync(queryInfo);

    [Authorize(Roles = "Admin")]
    [HttpGet("byUser/{userId}")]
    public async Task<QueryResult<Payment>> GetByUser(Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetPaidPaymentsByUserIdAsync(userId, queryInfo);
    }

    [Authorize]
    [HttpGet("byUser")]
    public async Task<QueryResult<Payment>> GetByUser([FromQuery] QueryInfo queryInfo)
    {
        var userId = HttpContext.GetCurrentUserId();
        return await _service.GetPaidPaymentsByUserIdAsync(userId, queryInfo);
    }

    [Authorize]
    [HttpPost("buy")]
    public async Task<IActionResult> BuyPackage([FromBody] BuyPackageModel model)
    {
        var userId = HttpContext.GetCurrentUserId();
        var result = await _service.BuyPackageAsync(userId, model.PackageId);
        return Ok(result);
    }

    // webhook k duoc de Authorize
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WebhookType payload)
    {
        await _service.HandleWebhookAsync(payload);
        return Ok(new { success = true });
    }
}
