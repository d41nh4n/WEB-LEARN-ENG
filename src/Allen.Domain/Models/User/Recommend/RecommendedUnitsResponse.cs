namespace Allen.Domain;

public class RecommendedUnitsResponse
{
	public string? Skill { get; set; }
	public List<UnitItem> Units { get; set; } = [];
}

public class UnitItem
{
	public Guid Id { get; set; }
	public string Title { get; set; } = null!;
	public string? Description { get; set; }
	public string? Level { get; set; }
}
