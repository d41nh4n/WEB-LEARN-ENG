namespace Allen.API;

public class PermissionsMiddleware
{
	private readonly RequestDelegate _next;

	public PermissionsMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		var _permissionService = context.RequestServices.GetRequiredService<IPermissionService>();

		var endpoint = context.GetEndpoint();
		if (endpoint == null)
		{
			await _next(context);
			return;
		}

		var permissionAttributes = endpoint.Metadata.GetOrderedMetadata<RequirePermissionsAttribute>();

		if (permissionAttributes == null || !permissionAttributes.Any())
		{
			await _next(context);
			return;
		}

		var requiredPermissions = permissionAttributes
			.SelectMany(attr => attr.RequiredPermissions)
			.Distinct()
			.ToList();

		var user = context.User;

		var userIdStr = user.FindFirst("Id")?.Value;
		if (!Guid.TryParse(userIdStr, out var userId))
		{
			throw new UnauthenticatedException("Invalid user token.");
		}

		var userPermissions = await _permissionService.GetPermissionsAsync(userId);

		var hasAllPermissions = requiredPermissions.All(p => userPermissions.Contains(p));

		if (!hasAllPermissions)
		{
			throw new ForbiddenException(ErrorMessageBase.Forbidden);
		}

		await _next(context);
	}
}
