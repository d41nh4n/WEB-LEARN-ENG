namespace Allen.Domain;

[Table("Feedbacks")]
public class FeedbackEntity : EntityBase<Guid>
{
	public Guid UserId { get; set; }	
	public Guid CategoryId { get; set; }
	public string Title { get; set; } = null!;
	public string Description { get; set; } = null!;
	public string? Image { get; set; }
}
