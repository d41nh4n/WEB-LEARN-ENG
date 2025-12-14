namespace Allen;

[RegisterService(typeof(IPermissionService))]
public class PermissionService : IPermissionService
{
	private readonly IRolesRepository _rolesRepository;
	private readonly IMemoryService _cacheService;
	private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

	public PermissionService(IRolesRepository rolesRepository, IMemoryService cacheService)
	{
		_rolesRepository = rolesRepository;
		_cacheService = cacheService;
	}

	public async Task<List<string>> GetPermissionsAsync(Guid userId)
	{
		var key = $"permissions:user:{userId}";

		return await _cacheService.GetOrSetAsync(key, async () =>
		{
			var permissions = await _rolesRepository.GetPermissionsForUserAsync(userId);
			return permissions.Select(p => $"{p.Resource}:{p.Action}").ToList();
		}, _cacheDuration) ?? [];
	}

	public Task InvalidatePermissionsAsync(Guid userId)
	{
		var key = $"permissions:user:{userId}";
		_cacheService.Remove(key);
		return Task.CompletedTask;
	}
}
