namespace Allen;

public interface IPermissionService
{
	Task<List<string>> GetPermissionsAsync(Guid userId);
	Task InvalidatePermissionsAsync(Guid userId);
}
