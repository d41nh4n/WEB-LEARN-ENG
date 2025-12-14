namespace Allen.Common;

public class ForbiddenException : DomainException
{
	public ForbiddenException(string message)
	   : base("Forbidden", message)
	{
	}
}
