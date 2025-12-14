namespace Allen.Domain;

public class ChatMessage
{
	public string Role { get; set; } = "user";
	public string? Content { get; set; }
}
