namespace Allen.Domain;

public class SubmitSpeakingIeltsModel
{
	public string Question { get; set; } = null!;
	public IFormFile AudioFile { get; set; } = null!;
	public string ReferenceText { get; set; } = string.Empty;
	public string? Accent { get; set; } = "US";
}
