namespace Allen.Common;

/// <summary>
/// DTO cho Response của một kết quả Cursor Pagination
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của item</typeparam>
public class CursorQueryResult<T>
{
    /// <summary>
    /// Danh sách data của trang này
    /// </summary>
    public List<T> Data { get; set; } = [];

    /// <summary>
    /// Total Card
    /// </summary>
    public int? Total { get; set; }

    /// <summary>
    /// Con trỏ (cursor) để lấy trang tiếp theo.
    /// Sẽ là null nếu đây là trang cuối cùng.
    /// </summary>
    public long? NextCursor { get; set; }
}