namespace Allen.Common;

public class ParameterInvalidException : DomainException
{
	public ParameterInvalidException(string message) : base("Invalid parameter", message)
	{
	}
}
