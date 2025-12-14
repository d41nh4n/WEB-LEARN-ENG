namespace Allen.Common;

public class CursorQueryInfo
{
    /// <summary>
    /// Số lượng item muốn lấy (ví dụ: 20)
    /// </summary>
    public int Limit { get; set; } = 20;

    /// <summary>
    /// Con trỏ (cursor) của item cuối cùng ở trang trước.
    /// Chúng ta sẽ dùng CreatedAt (dưới dạng Ticks) làm con trỏ.
    /// Sẽ là null nếu là trang đầu tiên.
    /// </summary>
    public long? AfterCursor { get; set; } = null;
}
