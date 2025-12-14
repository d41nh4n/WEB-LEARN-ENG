namespace Allen.Common;

public class NotFoundException : DomainException
{
	public NotFoundException(string message)
		: base("Not found", message)
	{
	}
}
