namespace Allen.Domain;

public interface ISoftDelete
{
	public bool? IsDeleted { get; set; }
	public DateTime? DeleteTime { get; set; }

	public void Undo()
	{
		IsDeleted = false;
		DeleteTime = null;
	}
}
