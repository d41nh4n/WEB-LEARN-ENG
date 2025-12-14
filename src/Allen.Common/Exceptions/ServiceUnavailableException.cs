namespace Allen.Common;

public class ServiceUnavailableException : DomainException
{
	public ServiceUnavailableException(string message)
		: base("Service Unavailable",message)
	{
	}
}
