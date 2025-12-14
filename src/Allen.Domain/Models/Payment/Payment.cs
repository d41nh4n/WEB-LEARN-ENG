namespace Allen.Domain;

public class Payment
{
    public Guid PaymentId { get; set; }
    public UserModels? User { get; set; }
    public PackageModels? Package { get; set; }
    public long OrderCode { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class UserModels
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
}

public class PackageModels
{
    public Guid PackageId { get; set; }
    public string? PackageName { get; set; }
}