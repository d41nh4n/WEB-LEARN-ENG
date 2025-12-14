namespace Allen.Domain;
[Table("Topics")]
public class TopicEntity : EntityBase<Guid>
{
    public string? TopicName { get; set; }
    public string? TopicDecription { get; set; }
    public ICollection<VocabularyEntity> Vocabularies { get; set; } = [];
}
