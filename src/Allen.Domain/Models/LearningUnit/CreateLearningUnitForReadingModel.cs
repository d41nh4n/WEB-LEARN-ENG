namespace Allen.Domain;

public class CreateLearningUnitForReadingModel
{
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Level { get; set; } = null!;
}
