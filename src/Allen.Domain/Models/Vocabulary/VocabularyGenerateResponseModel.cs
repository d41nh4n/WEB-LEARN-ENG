namespace Allen.Domain;

public class VocabularyGenerateResponseModel
{
    public List<VocabularyPreviewModel> Existing { get; set; } = [];
    public List<VocabularyPreviewModel> Generated { get; set; } = [];
    public List<InvalidVocabularyGenerateModel> Invalid { get; set; } = [];
}

