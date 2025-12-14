namespace Allen.Domain;

public class QuestionForSpeakingModel
{
	public Guid Id { get; set; }
	public Guid ModuleItemId { get; set; }
	public string? Label { get; set; }
	public string? Prompt { get; set; }
}
