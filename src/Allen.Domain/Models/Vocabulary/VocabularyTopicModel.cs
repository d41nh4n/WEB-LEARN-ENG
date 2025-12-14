namespace Allen.Domain;

public class VocabularyTopicModel
{   
    public Guid? Id { get; set; }
    public string? TopicName { get; set; }

    [JsonIgnore]
    public List<VocabularyEntity>? Vocabularies { get; set; }
}
