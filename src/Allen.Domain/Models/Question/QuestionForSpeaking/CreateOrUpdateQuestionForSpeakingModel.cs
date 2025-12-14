namespace Allen.Domain;

public class CreateOrUpdateQuestionForSpeakingModel
{
	public Guid ModuleItemId { get; set; }
	public string? QuestionType { get; set; }
	public string? Label { get; set; }
	public string Prompt { get; set; } = null!;
}
