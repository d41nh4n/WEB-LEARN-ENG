namespace Allen.Domain;

public class VocabularyMeaningModel
{
    public Guid? Id { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Pronunciation { get; set; }
    public string? Audio { get; set; }
    public string? DefinitionEN { get; set; }
    public string? DefinitionVN { get; set; }
    public string? Example1 { get; set; }
    public string? Example2 { get; set; }
    public string? Example3 { get; set; }
}
