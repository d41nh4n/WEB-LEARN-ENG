namespace Allen.Domain;

public class ReadingPassageSumaryModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int? EstimatedReadingTime { get; set; }
    //public int TotalPassages { get; set; }
    public int TotalQuestions { get; set; }
}
