namespace Allen.Domain;

public class VocabularyGenerateAiModel
{
    public string? Word { get; set; }
    public bool Skip { get; set; }
    public string? Level { get; set; }
    public string? TopicName { get; set; }

    public List<VocabularyMeaningAiModel>? VocabularyMeaningModels { get; set; }
}
