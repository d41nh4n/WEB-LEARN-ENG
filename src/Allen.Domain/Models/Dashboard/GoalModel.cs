namespace Allen.Domain;

public class GoalModel
{
	public Guid UserId { get; set; }
	public decimal Overall { get; set; }
	public decimal Reading { get; set; }
	public decimal Listening { get; set; }
	public decimal Writing { get; set; }
	public decimal Speaking { get; set; }
}
