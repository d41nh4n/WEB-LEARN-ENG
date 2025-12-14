

namespace Allen.Infrastructure;

[RegisterService(typeof(IDeckRepository))]
public class DeckRepository(SqlApplicationDbContext context)
    : RepositoryBase<DeckEntity>(context), IDeckRepository
{
    private readonly SqlApplicationDbContext _context = context;
    public async Task<DeckModel> GetDeckModelById(Guid deckId)
    {
        return await _context.Decks
            .AsNoTracking()
            .Where(d => d.Id == deckId)
            .Select(d => new DeckModel
            {
                Id = d.Id,
                DeckName = d.DeckName,
                Description = d.Description,
                IsPublic = d.IsPublic,
                TotalUsersUsing = d.TotalUsersUsing,
                IsClone = d.IsClone,
                Level = d.Level.ToString(),
                TotalFlashCard = d.FlashCards == null ? 0 : d.FlashCards.Count(),
            })
            .SingleOrDefaultAsync() ?? throw new NotFoundException(
                 ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(DeckEntity), deckId));
    }

    public async Task<bool> CheckDeckOwnerAsync(Guid deckId, Guid userId)
    {
        return await _context.Decks
            .AsNoTracking()
        .AnyAsync(d => d.Id == deckId && d.UserCreateId == userId);
    }

    public async Task<DeckPropsModel> GetDeckPropsAsync(Guid deckId)
    {
        var deckPropsQuery = _context.Decks
            .AsNoTracking()
            .Where(d => d.Id == deckId)
            .Select(d => new DeckPropsModel
            {
                IsPublic = d.IsPublic,
                NormalModeEnabled = d.NormalModeEnabled,
                NumberFlashCardsPerSession = d.NumberFlashCardsPerSession,
                DesiredRetention = d.DesiredRetention
            });

        var result = await deckPropsQuery.SingleOrDefaultAsync();

        return result == null
            ? throw new NotFoundException(
                ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(DeckEntity), deckId))
            : result;
    }
    public Task<List<DeckModel>> GetDeckModelByUserId(Guid userId)
    {
        return _context.Decks
            .AsNoTracking()
            .Where(d => d.UserCreateId == userId)
            .Select(d => new DeckModel
            {
                Id = d.Id,
                DeckName = d.DeckName,
                Description = d.Description,
                IsPublic = d.IsPublic,
                TotalUsersUsing = d.TotalUsersUsing,
                IsClone = d.IsClone,
                Level = d.Level.ToString(),
                TotalFlashCard = d.FlashCards.Count(),
                NumberFlashcardsPerSession = d.NumberFlashCardsPerSession
            })
            .ToListAsync() ?? throw new NotFoundException(
                 ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(DeckEntity), ""));
    }
}