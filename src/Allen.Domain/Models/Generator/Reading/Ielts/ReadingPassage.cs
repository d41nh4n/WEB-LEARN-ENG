namespace Allen.Domain;

public class ReadingPassage
{
	public Guid PassageId { get; set; }
	public string Content { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string Difficulty { get; set; } = string.Empty; // A1, A2, B1, B2, C1, C2
}
