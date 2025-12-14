namespace Allen.Domain;

public class AddPointsModel
{
    public Guid UserId { get; set; }
    public int Points { get; set; }
    public string? Description { get; set; } = "Bonus";
}