namespace Allen.Domain;

public class TopicModel
{
	public Guid Id { get; set; }
	public string TopicName { get; set; } = null!;
	public string TopicDecription { get; set; } = null!;
}
