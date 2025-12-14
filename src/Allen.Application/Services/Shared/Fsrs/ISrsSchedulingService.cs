namespace Allen.Application;

/// <summary>
/// Interface định nghĩa tất cả các hàm tính toán
/// cho "Thuật toán SRS Đơn Giản (S/D)"
/// </summary>
public interface ISRSService
{
    // ========================================
    // A. NHÓM HÀM LẬP LỊCH CỐT LÕI
    // (Dùng trong mỗi lần review)
    // ========================================

    /// <summary>
    /// (Sửa từ Công thức 1) Tính Stability MỚI.
    /// Logic đã được sửa để S tăng khi Nhớ, và phụ thuộc vào D.
    /// </summary>
    /// <param name="currentStability">Stability hiện tại (S_old)</param>
    /// <param name="newDifficulty">Difficulty mới (D_new - vừa được tính)</param>
    /// <param name="rating">Rating (0-3) người dùng nhấn</param>
    /// <returns>Stability mới (S_new)</returns>
    double CalculateNewStability(double currentStability, double newDifficulty, RatingLearningCard rating);

    /// <summary>
    /// (Giữ từ Công thức 2) Tính Difficulty MỚI.
    /// </summary>
    /// <param name="currentDifficulty">Difficulty hiện tại (D_old)</param>
    /// <param name="rating">Rating (0-3)</param>
    /// <param name="beta">Tốc độ điều chỉnh D (ví dụ: 0.15)</param>
    /// <returns>Difficulty mới (D_new), kẹp trong [1.0, 10.0]</returns>
    double CalculateNewDifficulty(double currentDifficulty, RatingLearningCard rating, double beta = 0.15);

    /// <summary>
    /// (Giữ từ Công thức 5) Tính Interval (khoảng cách) TỐI ƯU.
    /// </summary>
    /// <param name="stability">Stability mới (S_new)</param>
    /// <param name="desiredRetention">Tỷ lệ duy trì mong muốn (ví dụ: 0.9)</param>
    /// <returns>Số ngày làm tròn đến lần ôn tập tiếp theo</returns>
    int CalculateOptimalInterval(double stability, double desiredRetention = 0.9);

    /// <summary>
    /// (Sửa từ Công thức 8) Tính Stability BAN ĐẦU cho thẻ mới.
    /// </summary>
    /// <param name="firstRating">Rating (0-3) lần đầu nhấn</param>
    /// <returns>Stability ban đầu</returns>
    double CalculateInitialStability(RatingLearningCard firstRating);

    /// <summary>
    /// (Giữ từ Công thức 9) Tính Difficulty BAN ĐẦU cho thẻ mới.
    /// </summary>
    /// <param name="firstRating">Rating (0-3) lần đầu nhấn</param>
    /// <returns>Difficulty ban đầu</returns>
    double CalculateInitialDifficulty(RatingLearningCard firstRating);

    // ========================================
    // B. NHÓM HÀM TIỆN ÍCH & PHÂN TÍCH
    // (Dùng cho thống kê, sắp xếp, v.v.)
    // ========================================

    /// <summary>
    /// (Giữ từ Công thức 3) Tính Retention (xác suất nhớ).
    /// </summary>
    /// <param name="daysSinceLastReview">Số ngày đã trôi qua</param>
    /// <param name="stability">Stability của thẻ</param>
    /// <returns>Xác suất (0.0 - 1.0)</returns>
    double CalculateRetention(int daysSinceLastReview, double stability);

    /// <summary>
    /// (Giữ từ Công thức 6) Tính Độ ưu tiên (Priority).
    /// </summary>
    /// <param name="retention">Retention hiện tại</param>
    /// <param name="difficulty">Difficulty của thẻ</param>
    /// <param name="daysOverdue">Số ngày bị quá hạn</param>
    /// <returns>Điểm ưu tiên (càng cao càng gấp)</returns>
    double CalculatePriority(double retention, double difficulty, int daysOverdue = 0);

    /// <summary>
    /// (Giữ từ Công thức 7) Dự đoán Retention tương lai.
    /// </summary>
    /// <param name="stability">Stability của thẻ</param>
    /// <param name="daysInFuture">Số ngày muốn dự đoán</param>
    /// <returns>Xác suất (0.0 - 1.0)</returns>
    double PredictFutureRetention(double stability, int daysInFuture);

    /// <summary>
    /// (Giữ từ Công thức 11) Tính Tốc độ suy giảm (Decay Rate).
    /// </summary>
    /// <param name="stability">Stability của thẻ</param>
    /// <returns>Tốc độ suy giảm</returns>
    double CalculateDecayRate(double stability);

    /// <summary>
    /// (Giữ từ Công thức 12) Tính số ngày để đạt Retention mục tiêu.
    /// </summary>
    /// <param name="stability">Stability của thẻ</param>
    /// <param name="targetRetention">Retention mục tiêu (ví dụ: 0.8)</param>
    /// <returns>Số ngày (làm tròn)</returns>
    int CalculateDaysToTargetRetention(double stability, double targetRetention);
}