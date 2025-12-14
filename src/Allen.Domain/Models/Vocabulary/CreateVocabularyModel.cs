namespace Allen.Domain;

public class CreateVocabularyModel
{
    public string Word { get; set; } = null!;
    public string? Level { get; set; }
    public Guid TopicId { get; set; }
    public List<CreateVocabularyMeaningModel>? VocabularyMeaningModels { get; set; }
}