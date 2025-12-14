namespace Allen.Domain;

public class CreateOrUpdateFlashCardWithFileModel
{
    public string? TextFrontCard { get; set; } = string.Empty;
    public string? TextBackCard { get; set; } = string.Empty;
    public IFormFile? FrontImgFile { get; set; }
    public IFormFile? BackImgFile { get; set; }
    public IFormFile? FrontAudioFile { get; set; }
    public IFormFile? BackAudioFile { get; set; }
    public string? Hint { get; set; } = string.Empty;
    public string? PersonalNotes { get; set; } = string.Empty;
}
