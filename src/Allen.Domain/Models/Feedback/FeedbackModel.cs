namespace Allen.Domain;

public class FeedbackModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string UserName { get; set; } = null!;
	public Guid CategoryId { get; set; }
	public string CategoryName { get; set; } = null!;
	public string Title { get; set; } = null!;
	public string Description { get; set; } = null!;
	public string? Image { get; set; }
}
