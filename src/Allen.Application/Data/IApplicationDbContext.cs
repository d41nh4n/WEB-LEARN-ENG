using Microsoft.EntityFrameworkCore;

namespace Allen.Application;

public interface IApplicationDbContext
{
	public DbSet<UserEntity> Customers { get; set; }
	//public DbSet<Role> Roles { get; set; }
	Task<int> SaveChangesAsync();
}
