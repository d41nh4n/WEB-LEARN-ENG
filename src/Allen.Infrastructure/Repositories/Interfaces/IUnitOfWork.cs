namespace Allen.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    IRepositoryBase<TEntity> Repository<TEntity>() where TEntity : EntityBase<Guid>;
    Task ExecuteWithTransactionAsync(Func<Task> operation);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task<bool> SaveChangesAsync();
}
