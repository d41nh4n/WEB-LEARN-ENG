namespace Allen.Domain.Entities;
[Table("UserDeck")]
public class UserDeckEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }

    public UserEntity? User { get; set; }

    public Guid DeckId { get; set; }

    public DeckEntity? Deck { get; set; }

    public bool CanEdit { get; set; } = false;

    public bool IsFavorite { get; set; } = false;

    public DateTime LastAccessedAt { get; set; }

    public AccessTypeDeckEnum AccessType { get; set; } = AccessTypeDeckEnum.Owner;
}