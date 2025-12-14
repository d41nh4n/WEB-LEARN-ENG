namespace Allen.Domain;

public class CreatePostModel
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public string? Privacy { get; set; }
    public List<IFormFile>? Images { get; set; }
}
