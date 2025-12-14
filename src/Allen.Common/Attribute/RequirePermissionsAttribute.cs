namespace Allen.Common;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionsAttribute : Attribute
{
	public List<string> RequiredPermissions { get; }

	public RequirePermissionsAttribute(string resource, string action)
	{
		RequiredPermissions = new() { $"{resource}:{action}" };
	}

	public RequirePermissionsAttribute(params string[] permissions)
	{
		RequiredPermissions = permissions.ToList();
	}
}
