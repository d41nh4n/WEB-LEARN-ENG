namespace Allen.Domain;

public class CreateOrUpdateTopicModel
{
	public string TopicName { get; set; } = null!;
	public string? TopicDecription { get; set; }
	public string SkillType { get; set; } = null!;
}
