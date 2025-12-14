
namespace Allen.Infrastructure;

[RegisterService(typeof(IReviewFLHistoryRepository))]
public class ReviewFLHistoryRepository(SqlApplicationDbContext context) : RepositoryBase<ReviewFLHistoryEntity>(context), IReviewFLHistoryRepository
{
    private readonly SqlApplicationDbContext _context = context;
    public async Task<int> GetReviewHistoryByUserIdToday(Guid userId)
    {
        var todayStartUtc = DateTime.UtcNow.Date;
        // Xác định giới hạn trên: 00:00:00 UTC của ngày tiếp theo
        var tomorrowStartUtc = todayStartUtc.AddDays(1);

        // Lưu ý: Dùng .Include/.ThenInclude để đảm bảo EF Core có thể tạo truy vấn JOIN
        // nhằm lọc theo Deck.UserCreateId.

        var totalReviewsToday = await _context.ReviewFLHistory
            .Include(r => r.FlashCardState)
                .ThenInclude(fcs => fcs.FlashCard)
                    .ThenInclude(fc => fc.Deck)
            .AsNoTracking()
            .Where(r =>
                // Lọc theo người tạo Deck (Giả định: người dùng chỉ ôn tập thẻ từ Deck họ tạo)
                r.FlashCardState.FlashCard.Deck.UserCreateId == userId &&

                // Lọc theo khoảng thời gian: >= 00:00:00 hôm nay VÀ < 00:00:00 ngày mai
                r.ReviewDate >= todayStartUtc &&
                r.ReviewDate < tomorrowStartUtc
            )
            .CountAsync(); // Thực thi truy vấn và trả về kết quả đếm

        return totalReviewsToday;
    }
}
