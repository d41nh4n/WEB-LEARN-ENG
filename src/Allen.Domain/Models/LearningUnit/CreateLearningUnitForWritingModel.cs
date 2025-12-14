namespace Allen.Domain;

public class CreateLearningUnitForWritingModel
{
    public Guid? CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Level { get; set; }
}
