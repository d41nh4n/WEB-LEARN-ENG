namespace Allen.Domain;

public class CreateOrUpdateFeedbackModel
{
	public Guid CategoryId { get; set; }
	public string Title { get; set; } = null!;
	public string Description { get; set; } = null!;
	public IFormFile? Image { get; set; }
}
