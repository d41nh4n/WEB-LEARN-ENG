namespace Allen.Domain;

public class UpdateReadingPassagesModel
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int? EstimatedReadingTime { get; set; }
    public int? ReadingPassageNumber { get; set; }
}