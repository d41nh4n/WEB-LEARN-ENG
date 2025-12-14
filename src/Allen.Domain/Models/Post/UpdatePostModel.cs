using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Allen.Domain;

public class UpdatePostModel
{
    [BindNever]
    public Guid UserId { get; set; }
    public string? Content { get; set; }
    public string? Privacy { get; set; }
    public List<string>? Medias { get; set; } = new();
    public List<IFormFile>? Images { get; set; }
}
