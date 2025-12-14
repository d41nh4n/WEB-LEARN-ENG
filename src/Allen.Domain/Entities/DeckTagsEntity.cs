namespace Allen.Domain.Entities;
[Table("DeckTags")]
public class DeckTagsEntity : EntityBase<Guid>
{
    public Guid DeckId { get; set; }
    public DeckEntity? Deck { get; set; }
    public Guid TagId { get; set; }
    public TagEntity? Tag { get; set; }
}
