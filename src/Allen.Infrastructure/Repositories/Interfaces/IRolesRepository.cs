namespace Allen;

public interface IRolesRepository : IRepositoryBase<RoleEntity>
{
	Task<List<Role>> GetAllWithPagingAsync(QueryInfo queryInfo);
	Task<List<Permission>> GetPermissionsForUserAsync(Guid userId);
	Task<List<Role>> GetRoleForUserAsync(Guid userId);
}
