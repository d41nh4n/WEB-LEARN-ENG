namespace Allen.Domain;

public class VocabularyModel
{
    public Guid Id { get; set; }
    public VocabularyTopicModel? Topic { get; set; }
    public string? Word { get; set; } = null!;
    public string? Level { get; set; }
    public IEnumerable<VocabularyMeaningModel>? VocabularyMeanings { get; set; }
}
