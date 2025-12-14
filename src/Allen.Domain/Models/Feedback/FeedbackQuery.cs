namespace Allen.Domain;

public class FeedbackQuery
{
	public Guid? CategoryId { get; set; }
	public string? UserName { get; set; }
	public DateTime? CreateAt { get; set; } 
}
