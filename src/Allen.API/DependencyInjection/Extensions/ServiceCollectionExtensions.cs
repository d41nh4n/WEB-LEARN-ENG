using MediatR;
using YoutubeTranscriptApi;

namespace Allen.API;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<YouTubeTranscriptApi>();
        services.AddTransient<ExceptionHandlingMiddleware>();
		services.Configure<OpenAIOptions>(configuration.GetSection("OpenAI"));
		services.AddHttpClient("OpenAIClient");

		services.AddAuthorization();
		return services;
	}
	public static WebApplication UseApiServices(this WebApplication app)
	{
		app.UseMiddleware<ExceptionHandlingMiddleware>();
		return app;
	}
	public static IServiceCollection AddConfigureMediatR(this IServiceCollection services)
	=> services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly))
	.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
	.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehavior<,>))
	.AddTransient(typeof(IPipelineBehavior<,>), typeof(TrackingPipelineBehavior<,>));
}
