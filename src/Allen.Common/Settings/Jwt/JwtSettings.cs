namespace Allen.Common;

public class JwtSettings
{
	public string SecretKey { get; set; } = string.Empty;
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
	public int AccessTokenExpiration { get; set; }
	public int RefreshTokenExpiration { get; set; }
}
