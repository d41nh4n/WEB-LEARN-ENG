namespace Allen.Domain;

[Table("UserPointTransactions")]
public class UserPointTransactionEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    public Guid PackageId { get; set; }
    public PackageEntity? Package { get; set; }

    public Guid? PaymentId { get; set; } // Thanh toán PayOS
    public PaymentEntity? Payment { get; set; }

    public int PointsChanged { get; set; } // +100 khi mua
    public int NewTotal { get; set; } // Tổng điểm sau giao dịch

    [MaxLength(255)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? ActionType { get; set; } // "BUY_PACKAGE", "BONUS"
}
