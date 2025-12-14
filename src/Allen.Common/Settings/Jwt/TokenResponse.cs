namespace Allen.Common;

public class TokenResponse
{
	public string AccessToken { get; set; } = string.Empty;
	public int ExpiresIn { get; set; }
}
