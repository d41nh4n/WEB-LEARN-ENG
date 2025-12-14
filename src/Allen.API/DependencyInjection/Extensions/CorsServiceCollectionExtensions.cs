namespace Allen.API;

public static class CorsServiceCollectionExtensions
{
	public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
	{
		services.AddCors(opt =>
		{
			opt.AddDefaultPolicy(builder =>
			{
				builder
					   .AllowAnyMethod()
					   .SetPreflightMaxAge(TimeSpan.FromDays(1))
					   .SetIsOriginAllowed(origin => true)
					   .AllowCredentials()
					   .AllowAnyHeader()
						;
			});
		});
		return services;
	}
}
