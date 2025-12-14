namespace Allen.Domain;

public class VocabularyPreviewModel
{
    public string Word { get; set; } = null!;
    public VocabularyTopicModel? Topic { get; set; }
    public string Level { get; set; } = null!;
    public bool IsExisting { get; set; } = true;
    public List<VocabularyMeaningModel> Meanings { get; set; } = [];
}

