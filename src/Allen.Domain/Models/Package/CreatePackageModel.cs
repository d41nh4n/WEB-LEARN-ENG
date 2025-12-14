namespace Allen.Domain;

public class CreatePackageModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Points { get; set; }
    public bool IsActive { get; set; }
}
