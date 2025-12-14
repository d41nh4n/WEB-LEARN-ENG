using Allen.Domain.Entities;

namespace Allen.Domain;

[Table("Tags")]
public class TagEntity : EntityBase<Guid>
{
    public string? NameTag { get; set; }
    public string? Description { get; set; }
    public ICollection<VocabularyTagEntity> VocabularyTags { get; set; } = [];
    public ICollection<DeckEntity> DeckTags { get; set; } = [];
}
