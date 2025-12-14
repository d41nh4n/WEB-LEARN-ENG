

using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Allen.Infrastructure;

[RegisterService(typeof(IFlashCardStateRepository))]
public class FlashCardStateRepository(SqlApplicationDbContext context)
    : RepositoryBase<FlashCardStateEntity>(context), IFlashCardStateRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task DeleteByFlashCardIdAsync(Guid flashCardId)
    {
        var states = await _context.FlashCardStates
            .Where(x => x.FlashCardId == flashCardId)
            .ToListAsync();

        _context.FlashCardStates.RemoveRange(states);
    }

    public Task<List<FlashCardStateEntity>> GetFlashCardsStateByDeckIdAsync(Guid deckId)
    {
        return _context.FlashCardStates
            .Include(s => s.FlashCard)
            .Where(s => s.FlashCard.DeckId == deckId)
            .ToListAsync(); ;
    }
}

