namespace Allen.Domain;

public class Movie
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("genres")]
    public IEnumerable<string>? Genres { get; set; }
}
