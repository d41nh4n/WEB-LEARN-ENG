namespace Allen.Domain;

public class CategoryQuery
{
    public string? SkillType { get; set; }
    public QueryInfo QueryInfo { get; set; } = new QueryInfo();
}
