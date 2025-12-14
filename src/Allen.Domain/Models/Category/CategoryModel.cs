namespace Allen.Domain;

public class CategoryModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? SkillType { get; set; }
    public DateTime? CreateAt { get; set; }
}
