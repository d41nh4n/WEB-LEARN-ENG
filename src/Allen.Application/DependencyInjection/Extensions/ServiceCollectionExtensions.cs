using Azure.Storage.Blobs;
using Meilisearch;
using Quartz;
using StackExchange.Redis;
namespace Allen.Application;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.Configure<SightengineOptions>(
			configuration.GetSection("Sightengine"));
		services.AddHttpClient<INSFWDetectionService, SightengineNSFWService>();

		services.AddServicesFromAssembly(
			Assembly.GetEntryAssembly() ?? Assembly.Load(""),
			Assembly.Load("allen.application"),
			Assembly.Load("allen.infrastructure")
);
		services.AddBlobStorageService(configuration);
		services.AddEmailService(configuration);
		services.AddCacheService(configuration);
		services.AddMeiliSearchService(configuration);
		services.AddMemoryCache();
		services.AddBackgroundJobs(configuration);
		return services;
	}

	public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, params Assembly[] assemblies)
	{
		foreach (var assembly in assemblies)
		{
			if (assembly == null) continue;

			var types = assembly.GetTypes()
				.Where(type => type.IsClass && !type.IsAbstract && type.GetCustomAttribute<RegisterServiceAttribute>() != null)
				.ToList();

			services.ScanTypes(types);
		}
		return services;
	}

	public static IServiceCollection ScanTypes(this IServiceCollection services, List<Type> types)
	{
		foreach (var type in types)
		{
			var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();

			if (attribute == null)
			{
				continue;
			}

			// Register service
			switch (attribute.Lifetime)
			{
				case ServiceLifetime.Transient:
					services.AddTransient(attribute.ServiceType, type);
					break;
				case ServiceLifetime.Scoped:
					services.AddScoped(attribute.ServiceType, type);
					break;
				case ServiceLifetime.Singleton:
					services.AddSingleton(attribute.ServiceType, type);
					break;
			}
		}
		return services;
	}
	public static IServiceCollection AddBlobStorageService(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton(new BlobServiceClient(configuration.GetConnectionString(AppConstants.AzureBlobStorage)));

		services.AddScoped<IBlobStorageService, BlobStorageService>();
		return services;
	}
	public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
	{
		var mailSettingsSection = configuration.GetSection("MailSettings");
		services.Configure<EmailSettings>(mailSettingsSection);
		services.AddTransient<IEmailService, EmailService>();

		return services;
	}
	public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
	{
		var redisConnectionString = configuration.GetConnectionString(AppConstants.RedisConnection)
			?? throw new ParameterInvalidException("");

		var options = ConfigurationOptions.Parse(redisConnectionString);
		options.Ssl = true;                 // Upstash yêu cầu SSL
		options.AbortOnConnectFail = false; // tránh throw khi không connect ngay

		services.AddSingleton<IConnectionMultiplexer>(sp =>
			ConnectionMultiplexer.Connect(options));

		services.AddScoped<IRedisService, RedisService>();

		return services;
	}

	public static IServiceCollection AddMeiliSearchService(this IServiceCollection services, IConfiguration configuration)
	{
		var host = configuration["MeiliSearch:Host"];
		var masterKey = configuration["MeiliSearch:MasterKey"];

		if (string.IsNullOrEmpty(host))
			throw new InvalidOperationException("MeiliSearch:Host is not configured.");
		if (string.IsNullOrEmpty(masterKey))
			throw new InvalidOperationException("MeiliSearch:MasterKey is not configured.");

		services.AddSingleton(sp => new MeilisearchClient(host, masterKey));

		services.AddScoped(typeof(IMeiliSearchService<>), typeof(MeiliSearchService<>));

		return services;
	}
	public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddQuartz(q =>
		{
			var jobKey = new JobKey(nameof(NotificationJob));
			q.AddJob<NotificationJob>(opts => opts.WithIdentity(jobKey));

			q.AddTrigger(opts => opts
				.ForJob(jobKey)
				.WithIdentity("NotificationJob-trigger")
				.WithCronSchedule("0 19 02 * * ?", x => x
				.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
				)
			);
		});
		// Đăng ký background service cho Quartz
		services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

		return services;
	}
}
