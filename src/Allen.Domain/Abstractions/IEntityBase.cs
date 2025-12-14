namespace Allen.Domain;
public interface IEntityBase<T> : IEntity
{
	public T Id { get; set; }
}

public interface IEntity
{
	public DateTime? CreatedAt { get; set; }
	public DateTime? LastModified { get; set; }
}
