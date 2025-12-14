namespace Allen.Domain;
[Table("VocabularyTags")]
public class VocabularyTagEntity : EntityBase<Guid>
{
    public Guid VocabularyId { get; set; }
    public VocabularyEntity? Vocabulary { get; set; } 
    public Guid TagId { get; set; }
    public TagEntity? Tag { get; set; }
}
