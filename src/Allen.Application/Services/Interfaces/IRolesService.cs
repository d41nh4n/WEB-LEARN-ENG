namespace Allen.Application;

public interface IRolesService
{
	Task<List<Role>> GetRolesAsync();
	Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id);
	Task<List<Permission>> GetPermissionsForUserAsync(Guid userId);
	Task<List<Role>> GetRoleForUserAsync(Guid userId);
}
