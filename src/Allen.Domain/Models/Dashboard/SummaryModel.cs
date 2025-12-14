namespace Allen.Domain;

public class SummaryModel
{
	public DateTime Date { get; set; }
	public int ReadingCount { get; set; }
	public int ListeningCount { get; set; }
	public int WritingCount { get; set; }
	public int SpeakingCount { get; set; }
	public double TotalMinutes { get; set; }
}
