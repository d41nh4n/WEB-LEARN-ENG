namespace Allen.Domain;

public class ChatRequest
{
	public string? Model { get; set; }
	public List<ChatMessage> Messages { get; set; } = [];
	public double Temperature { get; set; } = 0.7;
}
