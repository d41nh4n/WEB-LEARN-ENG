namespace Allen.Common;

public class OperationResult
{
	public bool Success { get; set; }
	public string? Message { get; set; }
	public object? Data { get; set; }

	public static OperationResult SuccessResult(string? message = null, object? data = null)
	{
		return new OperationResult
		{
			Success = true,
			Message = message,
			Data = data
		};
	}

	public static OperationResult Failure(string message, object? data = null)
	{
		return new OperationResult
		{
			Success = false,
			Message = message,
			Data = data
		};
	}
}
