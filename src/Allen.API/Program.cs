using AspNetCoreRateLimit;

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", EnvironmentVariableTarget.Process);

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration
//	.SetBasePath(Directory.GetCurrentDirectory())
//	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
//	.AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddRateLimiting(builder.Configuration);

builder.Services.AddCorsPolicy()
                .AddJwtAuthentication(builder.Configuration)
                .AddFluentValidation()
                .AddAutoMapper(typeof(AutoMapperProfiles))
                .AddInfrastructureServices(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddApiServices(builder.Configuration);

builder.Services.AddConfigureMediatR();

builder.Services.AddSignalR();


builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddTransient<TokenValidationMiddleware>();


builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitializeDatabaseAsync();
}
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitializeDatabaseAsync();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();

app.UseIpRateLimiting();

app.UseMiddleware<PermissionsMiddleware>();

//app.MapAllHubs("/hubs");
app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllers();

try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during boostrapping.");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}