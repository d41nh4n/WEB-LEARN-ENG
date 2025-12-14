using Allen.Common; // Giả sử đây là nơi chứa Enum và Exception
using System;

namespace Allen.Application;

/// <summary>
/// Implementation của "Thuật toán SRS Đơn Giản (S/D)"
/// Đã được cập nhật đầy đủ validation.
/// </summary>
[RegisterService(typeof(ISRSService))]
public class SRSService : ISRSService
{
    // ========================================
    // A. NHÓM HÀM LẬP LỊCH CỐT LÕI
    // ========================================

    /// <summary>
    /// Tính Stability MỚI.
    /// </summary>
    public double CalculateNewStability(double currentStability, double newDifficulty, RatingLearningCard rating)
    {
        // === VALIDATION ===
        if (currentStability < 0)
            throw new ArgumentOutOfRangeException(nameof(currentStability), "Stability không thể là số âm.");

        if (newDifficulty < 1.0 || newDifficulty > 10.0)
            throw new ArgumentOutOfRangeException(nameof(newDifficulty), "Difficulty phải nằm trong khoảng [1.0, 10.0].");
        // ==================

        // Trường hợp 1: Quên (Forgot)
        if (rating == RatingLearningCard.Forgotten)
        {
            return Math.Max(1.0, currentStability * 0.5);
        }

        // Trường hợp 2: Nhớ (Hard, Good, Easy)
        double growthFactor = rating switch
        {
            RatingLearningCard.Hard => 1.5,
            RatingLearningCard.Good => 4.0,
            RatingLearningCard.Easy => 10,
            _ => 0.40
        };

        double newStability = currentStability * (1.0 + (growthFactor / newDifficulty));
        return Math.Max(0.5, newStability);
    }

    /// <summary>
    /// Tính Difficulty MỚI.
    /// </summary>
    public double CalculateNewDifficulty(double currentDifficulty, RatingLearningCard rating, double beta = 0.3)
    {
        // === VALIDATION ===
        if (currentDifficulty < 1.0 || currentDifficulty > 10.0)
            throw new ArgumentOutOfRangeException(nameof(currentDifficulty), "Difficulty hiện tại phải nằm trong khoảng [1.0, 10.0].");

        if (beta <= 0)
            throw new ArgumentOutOfRangeException(nameof(beta), "Beta (tốc độ học) phải là số dương.");
        // ==================

        int ratingValue = rating switch
        {
            RatingLearningCard.Forgotten => 0,
            RatingLearningCard.Hard => 1,
            RatingLearningCard.Good => 2,
            RatingLearningCard.Easy => 3,
            _ => 2
        };

        double newDifficulty = currentDifficulty + beta * (2 - ratingValue);
        return Math.Clamp(newDifficulty, 1.0, 10.0);
    }

    /// <summary>
    /// Tính Interval (khoảng cách) TỐI ƯU.
    /// </summary>
    public int CalculateOptimalInterval(double stability, double desiredRetention = 0.9)
    {
        // === VALIDATION ===
        if (desiredRetention <= 0 || desiredRetention >= 1)
            throw new ArgumentOutOfRangeException(nameof(desiredRetention), "DesiredRetention phải nằm trong khoảng (0, 1).");
        // ==================

        if (stability <= 0) return 1;

        double optimalInterval = -stability * Math.Log(desiredRetention);
        return Math.Max(1, (int)Math.Round(optimalInterval));
    }

    /// <summary>
    /// Tính Stability BAN ĐẦU cho thẻ mới.
    /// </summary>
    public double CalculateInitialStability(RatingLearningCard firstRating)
    {
        // (Không cần validation vì Enum đã bao quát tất cả các trường hợp)
        return firstRating switch
        {
            RatingLearningCard.Forgotten => 4.0,  // I = Round(4.0 * 0.105) = 0 -> 1
            RatingLearningCard.Hard => 10.0, // I = Round(10.0 * 0.105) = 1
            RatingLearningCard.Good => 24.0, // I = Round(24.0 * 0.105) = 3
            RatingLearningCard.Easy => 43.0, // I = Round(43.0 * 0.105) = 5
            _ => 24.0
        };
    }

    /// <summary>
    /// Tính Difficulty BAN ĐẦU cho thẻ mới.
    /// </summary>
    public double CalculateInitialDifficulty(RatingLearningCard firstRating)
    {
        // (Không cần validation vì Enum đã bao quát tất cả các trường hợp)
        return firstRating switch
        {
            RatingLearningCard.Forgotten => 9.0,
            RatingLearningCard.Hard => 7.0,
            RatingLearningCard.Good => 5.0,
            RatingLearningCard.Easy => 3.0,
            _ => 5.0
        };
    }

    // ========================================
    // B. NHÓM HÀM TIỆN ÍCH & PHÂN TÍCH
    // ========================================

    /// <summary>
    /// Tính Retention (xác suất nhớ).
    /// </summary>
    public double CalculateRetention(int daysSinceLastReview, double stability)
    {
        // === VALIDATION ===
        if (daysSinceLastReview < 0)
            throw new ArgumentOutOfRangeException(nameof(daysSinceLastReview), "Số ngày không thể là số âm.");
        // ==================

        if (stability <= 0) return 0;

        double retention = Math.Exp(-daysSinceLastReview / stability);
        return Math.Clamp(retention, 0.0, 1.0);
    }

    /// <summary>
    /// Tính Độ ưu tiên (Priority).
    /// </summary>
    public double CalculatePriority(double retention, double difficulty, int daysOverdue = 0)
    {
        // === VALIDATION ===
        if (retention < 0 || retention > 1)
            throw new ArgumentOutOfRangeException(nameof(retention), "Retention phải nằm trong khoảng [0, 1].");
        if (difficulty < 1.0 || difficulty > 10.0)
            throw new ArgumentOutOfRangeException(nameof(difficulty), "Difficulty phải nằm trong khoảng [1.0, 10.0].");
        if (daysOverdue < 0)
            throw new ArgumentOutOfRangeException(nameof(daysOverdue), "Số ngày quá hạn không thể là số âm.");
        // ==================

        double overdueMultiplier = 1.0 + Math.Min(daysOverdue / 7.0, 2.0);
        double priority = (1 - retention) * difficulty * overdueMultiplier;
        return priority;
    }

    /// <summary>
    /// Dự đoán Retention tương lai.
    /// </summary>
    public double PredictFutureRetention(double stability, int daysInFuture)
    {
        // === VALIDATION ===
        if (daysInFuture < 0)
            throw new ArgumentOutOfRangeException(nameof(daysInFuture), "Số ngày trong tương lai không thể là số âm.");
        // ==================

        if (stability <= 0) return 0;

        double futureRetention = Math.Exp(-daysInFuture / stability);
        return Math.Clamp(futureRetention, 0.0, 1.0);
    }

    /// <summary>
    /// Tính Tốc độ suy giảm (Decay Rate).
    /// </summary>
    public double CalculateDecayRate(double stability)
    {
        // === VALIDATION ===
        if (stability <= 0)
            throw new ArgumentOutOfRangeException(nameof(stability), "Stability phải là số dương để tính Decay Rate.");
        // ==================

        return -1.0 / stability;
    }

    /// <summary>
    /// Tính số ngày để đạt Retention mục tiêu.
    /// </summary>
    public int CalculateDaysToTargetRetention(double stability, double targetRetention)
    {
        // === VALIDATION ===
        if (targetRetention <= 0 || targetRetention >= 1)
            throw new ArgumentOutOfRangeException(nameof(targetRetention), "TargetRetention phải nằm trong khoảng (0, 1).");
        // ==================

        if (stability <= 0) return 0;

        double days = -stability * Math.Log(targetRetention);
        return Math.Max(0, (int)Math.Round(days));
    }
}