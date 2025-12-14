namespace Allen.Domain;

public class UnitStep
{
    public Guid Id { get; set; }
    public int StepIndex { get; set; }
    public string? Title { get; set; }
    public string? ContentJson { get; set; }
}
