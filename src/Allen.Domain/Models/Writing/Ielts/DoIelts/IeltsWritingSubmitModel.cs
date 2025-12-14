namespace Allen.Domain;

public class IeltsWritingSubmitModel
{
    public Guid WritingId { get; set; }
    public string WritingTaskType { get; set; } = null!;
	public string? Content { get; set; }
    public double? TimeSpent { get; set; }
}