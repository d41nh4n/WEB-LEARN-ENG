namespace Allen.Common;

public class InternalServerException : DomainException
{
	public InternalServerException(string title, string message)
		: base("InternalServer", message)
	{
	}
}
