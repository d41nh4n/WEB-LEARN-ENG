using Microsoft.AspNetCore.Builder;
namespace Allen.Infrastructure;
public static class DatabaseExtensions
{
	public static async Task InitializeDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var context = scope.ServiceProvider.GetRequiredService<SqlApplicationDbContext>();

		context.Database.MigrateAsync().GetAwaiter().GetResult();

		await SeedAsync(context);
	}

	private static async Task SeedAsync(SqlApplicationDbContext context)
	{
		await SeedRoleAsync(context);
		await SeedCustomerAsync(context);
		await SeedUserRolesAsync(context);
	}
	private static async Task SeedRoleAsync(SqlApplicationDbContext context)
	{
		if (!await context.Roles.AnyAsync())
		{
			await context.Roles.AddRangeAsync(InitialData.Roles);
			await context.SaveChangesAsync();
		}
	}
	private static async Task SeedCustomerAsync(SqlApplicationDbContext context)
	{
		if (!await context.Users.AnyAsync())
		{
			await context.Users.AddRangeAsync(InitialData.Users);
			await context.SaveChangesAsync();
		}
	}

	private static async Task SeedUserRolesAsync(SqlApplicationDbContext context)
	{
		if (!await context.UserRoles.AnyAsync())
		{
			await context.UserRoles.AddRangeAsync(InitialData.UserRoles);
			await context.SaveChangesAsync();
		}
	}
}

