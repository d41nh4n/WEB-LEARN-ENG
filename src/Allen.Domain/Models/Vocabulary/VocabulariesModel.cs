namespace Allen.Domain;

public class VocabulariesModel
{
    public Guid Id { get; set; }
    public string? Word { get; set; } = null!;
    public string? Level { get; set; }
}
