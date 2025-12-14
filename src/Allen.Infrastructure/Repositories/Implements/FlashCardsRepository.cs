namespace Allen.Infrastructure;

[RegisterService(typeof(IFlashCardsRepository))]
public class FlashCardsRepository(SqlApplicationDbContext context)
    : RepositoryBase<FlashCardEntity>(context), IFlashCardsRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public Task<QueryResult<FlashCardModel>> GetFlashCardsAsync(QueryInfo queryInfo)
    {
        throw new NotImplementedException();
    }

    public async Task<FlashCardEntity> GetFlashCardByIdAsync(Guid id)
    {
        var flashCard = await _context.FlashCards
            .AsNoTracking()
            .Include(fc => fc.CardState)
            .Include(fc => fc.Deck)
            .Select(fc => new FlashCardEntity
            {
                Id = fc.Id,
                DeckId = fc.DeckId,
                Deck = new DeckEntity
                {
                    Id = fc.Deck.Id,
                    DesiredRetention = fc.Deck.DesiredRetention,
                },
                FrontContent = fc.FrontContent,
                BackContent = fc.BackContent,
                Hint = fc.Hint,
                PersonalNotes = fc.PersonalNotes,
                IsSuspended = fc.IsSuspended,
                LastClonedDate = fc.LastClonedDate,
                CardState = new FlashCardStateEntity
                {
                    Id = fc.CardState != null ? fc.CardState.Id : Guid.Empty,
                    FlashCardId = fc.CardState != null ? fc.CardState.FlashCardId : Guid.Empty,
                    Stability = fc.CardState != null ? fc.CardState.Stability : 0,
                    Difficulty = fc.CardState != null ? fc.CardState.Difficulty : 0,
                    Repetition = fc.CardState != null ? fc.CardState.Repetition : 0,
                    Interval = fc.CardState != null ? fc.CardState.Interval : 0,
                    NextReviewDate = fc.CardState != null ? fc.CardState.NextReviewDate : null,
                    LastReviewedAt = fc.CardState != null ? fc.CardState.LastReviewedAt : null,
                    LastRating = fc.CardState != null ? fc.CardState.LastRating : null,
                    CreatedAt = fc.CreatedAt,
                }
            })
            .FirstOrDefaultAsync(fc => fc.Id == id);

        return flashCard ?? throw new NotFoundException(
            ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(FlashCardEntity), id));
    }

    public async Task<QueryResult<Guid>> GetFlashCardIdsByDeck(QueryInfo queryInfo, Guid deckId)
    {
        var query = _context.FlashCards
            .AsNoTracking()
            .Where(fc => fc.DeckId == deckId);

        // Áp dụng sorting
        if (!string.IsNullOrEmpty(queryInfo.OrderBy))
        {
            query = queryInfo.OrderType == OrderType.Ascending
                ? query.OrderBy(fc => EF.Property<object>(fc, queryInfo.OrderBy))
                : query.OrderByDescending(fc => EF.Property<object>(fc, queryInfo.OrderBy));
        }
        else
        {
            query = query.OrderBy(fc => fc.Id);
        }

        // Lấy tổng count trước khi paging (nếu cần)
        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        // Áp dụng paging và execute query
        var data = await query
            .Select(fc => fc.Id)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<Guid>
        {
            Data = data,
            TotalCount = totalCount
        };
    }

    public async Task<List<FlashCardEntity>> GetSrsStudyQueueAsync(Guid deckId, DateTime now, int newCardLimit)
    {
        // 1. Lấy tất cả thẻ ÔN TẬP (Review)
        // Chúng ta Include() CardState TRƯỚC KHI Select
        var reviewCardsQuery = _context.FlashCards
            .Include(fc => fc.CardState)
            .Where(fc => fc.DeckId == deckId &&
                         fc.CardState != null &&
                         fc.CardState.Repetition > 0 &&
                         fc.CardState.NextReviewDate <= now)
            .Select(fc => new
            {
                SortPriority = 1, // Ưu tiên 1
                CreatedAt = fc.CreatedAt, // Phải có cho Sắp xếp
                Card = fc // Entity (đã Include)
            });

        // 2. Lấy các thẻ MỚI (New)
        var newCardsQuery = _context.FlashCards
            .Include(fc => fc.CardState)
            .Where(fc => fc.DeckId == deckId &&
                         fc.CardState != null &&
                         fc.CardState.Repetition == 0)
            .OrderBy(fc => fc.CreatedAt) // Lấy thẻ mới nhất theo ngày tạo
            .Take(newCardLimit)
            .Select(fc => new
            {
                SortPriority = 2, // Ưu tiên 2
                CreatedAt = fc.CreatedAt,
                Card = fc // Entity (đã Include)
            });

        // 3. Gộp (UNION ALL = Concat) và Sắp xếp
        var combinedQuery = reviewCardsQuery.Concat(newCardsQuery)
            .OrderBy(x => x.SortPriority)   // 1. Đảm bảo Review (1) lên trước New (2)
            .ThenBy(x => x.CreatedAt);    // 2. Sắp xếp thẻ New theo ngày tạo

        // 4. Chọn lại Entity cuối cùng và thực thi
        return await combinedQuery
            .Select(x => new FlashCardEntity
            {
                Id = x.Card.Id,
                FrontContent = x.Card.FrontContent,
                BackContent = x.Card.BackContent,
                Hint = x.Card.Hint,
                PersonalNotes = x.Card.PersonalNotes,
                CardState = x.Card.CardState
            }) // Lấy lại FlashCardEntity
            .ToListAsync();
    }

    /// <summary>
    /// Lấy TẤT CẢ flashcard trong một bộ (cho Normal Mode).
    /// ĐÃ CẬP NHẬT để dùng Cursor Pagination.
    /// </summary>
    public async Task<(List<FlashCardEntity> Data, long? NextCursor)> GetAllCardsForDeckByCursorAsync(Guid deckId, long? afterCursor, int limit)
    {
        // 1. Bắt đầu với IQueryable (chưa sắp xếp)
        IQueryable<FlashCardEntity> query = _context.FlashCards
                .AsNoTracking()
                .Include(fc => fc.CardState)
                .Where(fc => fc.DeckId == deckId);

        // 2. Áp dụng bộ lọc Cursor NẾU CÓ
        if (afterCursor.HasValue)
        {
            // (Sửa lỗi so sánh DateTime? với DateTime)
            DateTime? cursorTimestamp = new DateTime(afterCursor.Value);

            query = query.Where(fc => fc.CreatedAt > cursorTimestamp);
        }

        // 3. Áp dụng SẮP XẾP và LẤY (Take) ở cuối cùng
        var items = await query
            .OrderBy(fc => fc.CreatedAt)
            .ThenBy(fc => fc.Id)// Sắp xếp Ở ĐÂY
            .Take(limit + 1) // Lấy nhiều hơn 1 item
            .ToListAsync();

        long? nextCursor = null;
        if (items.Count > limit)
        {
            // Lấy cursor (Ticks) của item cuối cùng (item thứ "limit")
            nextCursor = items[limit - 1].CreatedAt?.Ticks;

            // Xóa item thừa (item thứ "limit + 1")
            items.RemoveAt(limit);
        }

        return (items, nextCursor);
    }

    // --- CÁC HÀM KHÁC CHO SRS MODE (Giữ nguyên) ---

    /// <summary>
    /// Lấy các thẻ ÔN TẬP (Review) đã đến hạn.
    /// </summary>
    public async Task<List<FlashCardEntity>> GetReviewCardsAsync(Guid deckId, DateTime now)
    {
        return await _context.FlashCards
            .AsNoTracking()
            .Include(fc => fc.CardState)
            .Where(fc => fc.DeckId == deckId)
            .Where(fc => fc.CardState != null &&
                         fc.CardState.Repetition > 0 &&
                         fc.CardState.NextReviewDate <= now)
            .ToListAsync();
    }

    public async Task<List<FlashCardEntity>> GetAllCardsForDeckAsync(Guid deckId)
    {
        return await _context.FlashCards
            .AsNoTracking()
            .Include(fc => fc.CardState)
            .Include(fc => fc.Deck)
            .Where(fc => fc.DeckId == deckId)
            .ToListAsync();
    }

    public async Task<List<FlashCardEntity>> GetAllCardsForByIdsAsync(List<Guid> flashCardIds)
    {
        return await _context.FlashCards
            .AsNoTracking()
            .Include(fc => fc.CardState)
            .Include(fc => fc.Deck)
            .Where(fc => flashCardIds.Contains(fc.Id))
            .ToListAsync();
    }

    public async Task<List<FlashCardEntity>> GetFlashCardsInDeckByRelationVocabularyId(List<Guid> flashCardIds, Guid deckId)
    {
        return await _context.FlashCards
            .AsNoTracking()
            .Include(fc => fc.Deck)
            .Where(fc => flashCardIds.Contains(fc.RelationVocabularyCardId) && fc.DeckId == deckId)
            .ToListAsync() ?? [];
    }

    public async Task<List<FlashCardEntity>> GetFlashCardsOfUserHasRelationVocabularyCardId(Guid userId)
    {
        return await _context.FlashCards
                .AsNoTracking()
                .Include(fc => fc.CardState)
                .Include(fc => fc.Deck)
                .Where(fc => fc.Deck.UserCreateId == userId &&
                             fc.RelationVocabularyCardId != Guid.Empty)
                .ToListAsync();
    }

    public async Task<List<FlashCardEntity>> GetAllCardsForByIdsAndDeckIdAsync(List<Guid> flashCardIds, Guid deckId)
    {
        return await _context.FlashCards
               .AsNoTracking()
               .Include(fc => fc.CardState)
               .Include(fc => fc.Deck)
               .Where(fc => flashCardIds.Contains(fc.Id) && fc.DeckId == deckId)
               .ToListAsync();
    }

    public async Task<List<Guid>> GetListUserIdToNotificateStudyToday()
    {
        return await _context.FlashCards
            .Include(fc => fc.CardState)
            .Include(fc => fc.Deck)
            .AsNoTracking()
            .Where(fc => fc.CardState != null &&
                         fc.CardState.NextReviewDate <= DateTime.UtcNow)
            .Select(fc => fc.Deck.UserCreateId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetNumberNeedReviewTodayForUserIds(List<Guid> userIds)
    {
        var results = await _context.FlashCards
            .Where(fc => userIds.Contains(fc.Deck.UserCreateId) &&
                         fc.CardState != null &&
                         fc.CardState.NextReviewDate <= DateTime.UtcNow)
            .GroupBy(fc => fc.Deck.UserCreateId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        return results;
    }
}