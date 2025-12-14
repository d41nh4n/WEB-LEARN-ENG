namespace Allen.Domain;

public class CreateReadingParagraphModel
{
	[JsonIgnore]
    public int Order { get; set; }                // Thứ tự hiển thị
	public string OriginalText { get; set; } = null!;
	public string Transcript { get; set; } = null!;
}
