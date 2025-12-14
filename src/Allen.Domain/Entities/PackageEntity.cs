namespace Allen.Domain;

public class PackageEntity : EntityBase<Guid>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Points { get; set; }
    public bool IsActive { get; set; }
    public ICollection<UserPointTransactionEntity> UserPointTransactions { get; set; } = [];
}