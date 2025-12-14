namespace Allen.Domain;

[Table("Vocabularies")]
public class VocabularyEntity : EntityBase<Guid>
{
    public Guid? TopicId { get; set; }
    public TopicEntity? Topic { get; set; }
    public string Word { get; set; } = null!;
	public LevelType Level { get; set; }
    public ICollection<VocabularyMeaningEntity> VocabularyMeanings { get; set; } = [];
}