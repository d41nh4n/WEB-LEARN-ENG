namespace Allen;

[RegisterService(typeof(IRolesRepository))]
public class RolesRepository(
	SqlApplicationDbContext context,
	IMapper _mapper) : RepositoryBase<RoleEntity>(context), IRolesRepository
{
	private readonly SqlApplicationDbContext _context = context;
	public Task<List<Role>> GetAllWithPagingAsync(QueryInfo queryInfo)
	{
		throw new NotImplementedException();
	}
	public async Task<List<Permission>> GetPermissionsForUserAsync(Guid userId)
	{
		var userRoles = await _context.UserRoles
		.Where(ur => ur.UserId == userId)
		.Select(ur => ur.RoleId)
		.ToListAsync();

		var allRoleIds = new HashSet<Guid>(userRoles);

		var visited = new HashSet<Guid>();
		var queue = new Queue<Guid>(userRoles);

		while (queue.Count > 0)
		{
			var roleId = queue.Dequeue();
			if (visited.Contains(roleId)) continue;
			visited.Add(roleId);

			var children = await _context.RoleHierarchies
				.Where(rh => rh.ParentRoleId == roleId)
				.Select(rh => rh.ChildRoleId)
				.ToListAsync();

			foreach (var childId in children)
			{
				if (!allRoleIds.Contains(childId))
				{
					allRoleIds.Add(childId);
					queue.Enqueue(childId);
				}
			}
		}

		var permissions = await _context.RolePermissions
			.Where(rp => allRoleIds.Contains(rp.RoleId))
			.Select(rp => rp.Permission)
			.Distinct()
			.ToListAsync();

		var result = _mapper.Map<List<Permission>>(permissions);

		return result;
	}

	public async Task<List<Role>> GetRoleForUserAsync(Guid userId)
	{
		var userRoles = await _context.UserRoles
		 .Where(ur => ur.UserId == userId)
		 .Select(ur => new Role
		 {
			 Id = ur.Role!.Id,
			 Name = ur.Role.Name.ToString(),
		 }).ToListAsync();

		return userRoles;
	}
}

