namespace Allen.Common;

public class TokenInvalidException : DomainException
{
	public TokenInvalidException(string message) : base("Unauthorized", message)
	{
	}
}
