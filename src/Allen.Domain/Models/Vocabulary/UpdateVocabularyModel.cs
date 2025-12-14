namespace Allen.Domain;

public class UpdateVocabularyModel
{
    public string Word { get; set; } = null!;
    public string? Level { get; set; }
    public Guid TopicId { get; set; }
    public List<UpdateVocabularyMeaningModel>? VocabularyMeaningModels { get; set; }
    public List<Guid> UpdateTagsId { get; set; } = [];
}
