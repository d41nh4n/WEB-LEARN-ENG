namespace Allen.Domain;

public class FullResponse
{
    public Guid WritingSubmissionId { get; set; }
    public IeltsWritingResponse? IeltsWritingResponse { get; set; }
    public TestAttempt? TestAttempt { get; set; }
}

public class TestAttempt
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LearningUnitId { get; set; }
    public decimal OverallBand { get; set; }
    public double TimeSpent { get; set; }
}

public class IeltsWritingResponse
{
    public int TaskResponse { get; set; }
    public int CoherenceCohesion { get; set; }
    public int LexicalResource { get; set; }
    public int GrammaticalAccuracy { get; set; }
    public Feedback? Feedback { get; set; }
    public ExpandedFeedback? ExpandedFeedback { get; set; }
}

public class Feedback
{
    public string? TaskResponse { get; set; }
    public string? CoherenceCohesion { get; set; }
    public string? LexicalResource { get; set; }
    public string? GrammaticalAccuracy { get; set; }
}
public class ExpandedFeedback
{
	public string? TaskResponse { get; set; }
	public string? CoherenceCohesion { get; set; }
	public string? LexicalResource { get; set; }
	public string? GrammaticalAccuracy { get; set; }
}