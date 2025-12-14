namespace Allen.Common;

public interface IAppConfiguration
{
	GoogleSettings GetGoogleSettings();
	JwtSettings GetJwtSetting();
	string? GetSqlServerConnectionString();
	string? GetRedisConnectionString();
	public string? GetEnvironment();
	UrlSettings GetUrlSettings();
    AzureSpeechSettings GetAzureSpeechSetting();
}
