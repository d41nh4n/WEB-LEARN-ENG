using Allen.Common.Settings.Enum;

namespace Allen.Domain;

[Table("Decks")]
public class DeckEntity : EntityBase<Guid>
{
    // Basic information --------------------------------
    public string? DeckName { get; set; }
    public string? Description { get; set; }
    public Guid UserCreateId { get; set; }
    // Properties ---------------------------------------
    public int NumberFlashCardsPerSession { get; set; } = 20;
    public bool NormalModeEnabled { get; set; } = true; //option to enable/disable normal mode (normal: full card, un-normal: SRS)
    public bool IsPublic { get; set; } = false;
    public int TotalUsersUsing { get; set; } = 0;
    public bool IsClone { get; set; } = false;
    public DeckLevel Level { get; set; } = DeckLevel.Basic;

    // SRS Properties ---------------------------------------
    /// <summary>
    /// Tỷ lệ duy trì mong muốn (ví dụ: 0.9 = 90%).
    /// Dùng để tính toán khoảng cách ôn tập tối ưu.
    /// Nên nằm trong khoảng (0.8, 0.95).
    /// Nếu 0.8 thì khoảng cách dài hơn
    /// Nếu 0.95 thì khoảng cách ngắn hơn.
    /// Nên chọn giá trị cân bằng là 0.9
    /// </summary>
    public double DesiredRetention { get; set; } = 0.9;

    // Relations ---------------------------------------
    public UserEntity UserCreate { get; set; } = null!;
    public ICollection<FlashCardEntity> FlashCards { get; set; } = [];

    public static DeckEntity CloneFrom(DeckEntity source, Guid newOwnerId)
    {
        return new DeckEntity
        {
            Id = Guid.NewGuid(),
            DeckName = $"{source.DeckName} (Copy)",
            Description = source.Description,
            UserCreateId = newOwnerId, // Chủ sở hữu mới

            // Copy các cài đặt quan trọng
            NormalModeEnabled = source.NormalModeEnabled,
            NumberFlashCardsPerSession = source.NumberFlashCardsPerSession,
            DesiredRetention = source.DesiredRetention,
            Level = source.Level,

            // Reset các trạng thái về mặc định
            IsPublic = false, // Clone xong thì nên để Private
            IsClone = true,
            TotalUsersUsing = 0, // Reset người dùng

            CreatedAt = DateTime.UtcNow
        };
    }
}
