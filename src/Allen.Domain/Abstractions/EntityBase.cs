namespace Allen.Domain;

public abstract class EntityBase<T> : IEntityBase<T>
{
	public required T Id { get; set; }
	public DateTime? CreatedAt { get; set; }
	public DateTime? LastModified { get; set; }
}
