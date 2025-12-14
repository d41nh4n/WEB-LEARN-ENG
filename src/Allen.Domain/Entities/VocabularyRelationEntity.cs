namespace Allen.Domain;

[Table("VocabulariesRelation")]
public class VocabularyRelationEntity : EntityBase<Guid>
{
    public Guid SourceVocabId { get; set; }
    public VocabularyEntity? SourceVocab { get; set; } = null!;
    public Guid RelateVocabId { get; set; }
    public VocabularyEntity? RelateVocab { get; set; } = null!;
    public VocabRelationType? VocabRelationType { get; set; }
}
