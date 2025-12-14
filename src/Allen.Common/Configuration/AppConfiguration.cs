using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Allen.Common;

public class AppConfiguration(IConfiguration _configuration, IHostEnvironment _hostEnvironment) : IAppConfiguration
{
	public GoogleSettings GetGoogleSettings()
	{
		var clientId = _configuration["Authentication:Google:ClientId"];
		var clientSecret = _configuration["Authentication:Google:ClientSecret"];
		return new GoogleSettings
		{
			ClientId = clientId ?? "",
			ClientSecret = clientSecret ?? ""
		};
	}
	public JwtSettings GetJwtSetting()
	{
		var audience = _configuration["JwtSettings:Audience"];
		var issuer = _configuration["JwtSettings:Issuer"];
		var secretKey = _configuration["JwtSettings:SecretKey"];
		var accessTokenExpiration = _configuration["JwtSettings:AccessTokenExpiration"];
		var refreshTokenExpiration = _configuration["JwtSettings:RefreshTokenExpiration"];

		return new JwtSettings
		{
			Audience = audience ?? "",
			Issuer = issuer ?? "",
			SecretKey = secretKey ?? "",
			AccessTokenExpiration = int.Parse(accessTokenExpiration ?? ""),
			RefreshTokenExpiration = int.Parse(refreshTokenExpiration ?? "")
		};
	}
	public string? GetSqlServerConnectionString()
   => _configuration.GetConnectionString(AppConstants.SqlServerConnection) ?? throw new Exception("An unexpected error occurred.");

	public string? GetRedisConnectionString()
	=> _configuration.GetConnectionString(AppConstants.RedisConnection) ?? throw new Exception("An unexpected error occurred.");

	public string? GetEnvironment()
	{
		return _hostEnvironment.EnvironmentName;
	}

    public UrlSettings GetUrlSettings()
    {
		var environment = GetEnvironment();
		if(environment == "Development")
		{
            return new UrlSettings
            {
                BaseUrl = _configuration["UrlSettings:Production:BaseUrl"] ?? throw new Exception("Base URL in local is not configured."),
                ClientUrl = _configuration["UrlSettings:Production:ClientUrl"] ?? throw new Exception("Client URL in local is not configured.")
            };
        }
		else
		{
			return new UrlSettings
			{
				BaseUrl = _configuration["UrlSettings:Production:BaseUrl"] ?? throw new Exception("Base URL in production is not configured."),
				ClientUrl = _configuration["UrlSettings:Production:ClientUrl"] ?? throw new Exception("Client URL in production is not configured.")
			};
		}
    }

    public AzureSpeechSettings GetAzureSpeechSetting()
    {
        var key = _configuration["AzureConfig:AzureSpeech:Key"];
        var region = _configuration["AzureConfig:AzureSpeech:Region"];
        return new AzureSpeechSettings
        {
            Key = key ?? "",
            Region = region ?? ""
        };
    }
}

