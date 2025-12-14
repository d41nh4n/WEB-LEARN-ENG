namespace Allen.Domain;

[Table("Payments")]
public class PaymentEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    public Guid? PackageId { get; set; }
    public PackageEntity? Package { get; set; }

    public long OrderCode { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Currency { get; set; } = "VND";

    [MaxLength(100)]
    public string? PaymentLinkId { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    [MaxLength(200)]
    public string? CheckoutUrl { get; set; }

    public string? AccountNumber { get; set; }

    [MaxLength(10)]
    public string? Bin { get; set; }

    public string? QrCode { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public ICollection<UserPointTransactionEntity> UserPointTransactions { get; set; } = [];
}
