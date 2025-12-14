namespace Allen.Domain;
public class VocabularyOfUserModel
{
    public Guid Id { get; set; }
    public string? Word { get; set; } = null!;
    public string? Level { get; set; }
    public List<VocabularyMeaningModel>? VocabularyMeanings { get; set; }
}
