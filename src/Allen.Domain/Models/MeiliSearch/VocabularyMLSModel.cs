namespace Allen.Domain;

public class VocabularyMLSModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("word")]
    public string? Word { get; set; }
}
