namespace Allen.Domain;

public class UserPointTransaction
{
    public Guid TransactionId { get; set; }
    public UserModels? User { get; set; }
    public PackageModels? Package { get; set; }
    public int PointsChanged { get; set; }
    public int NewTotal { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}
