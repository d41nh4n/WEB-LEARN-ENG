namespace Allen.API;

internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;
	public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
		=> _logger = logger;

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (Exception e)
		{
			_logger.LogError(e, e.Message);
			await HandleExceptionAsync(context, e);
		}
	}

	private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
	{
		var statusCode = GetStatusCode(exception);

		var response = new
		{
			title = GetTitle(exception),
			status = statusCode,
			detail = GetDetail(exception),
			errors = GetErrors(exception)
		};

		httpContext.Response.ContentType = "application/json";

		httpContext.Response.StatusCode = statusCode;

		await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
	}
	private static int GetStatusCode(Exception exception) =>
		exception switch
		{
			FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
			BadRequestException => StatusCodes.Status400BadRequest,
			ParameterInvalidException => StatusCodes.Status400BadRequest,
			UnauthenticatedException => StatusCodes.Status401Unauthorized,
			NoAuthenticationHeaderException => StatusCodes.Status401Unauthorized,
			TokenInvalidException => StatusCodes.Status401Unauthorized,
			TokenExpiredException => StatusCodes.Status401Unauthorized,
			ForbiddenException => StatusCodes.Status403Forbidden,
			NotFoundException => StatusCodes.Status404NotFound,
			FormatException => StatusCodes.Status422UnprocessableEntity,
			ServiceUnavailableException => StatusCodes.Status503ServiceUnavailable,
			_ => StatusCodes.Status500InternalServerError
		};
	private static string GetDetail(Exception exception)
	   => exception switch
	   {
		   FluentValidation.ValidationException => "One or more propety was validated.",
		   _ => exception.Message
	   };
	private static object GetTitle(Exception exception)
	=> exception switch
	{
		DomainException applicationException => applicationException.Title,
		FluentValidation.ValidationException => "Validation failed",
		_ => "Server error"
	};

	private static List<object>? GetErrors(Exception exception)
	{
		if (exception is FluentValidation.ValidationException validationException)
		{
			return validationException.Errors
				.Select(e => new { e.PropertyName, e.ErrorMessage })
				.Cast<object>()
				.ToList();
		}

		if (exception is SqlException sqlException)
		{
			var transientErrorDescriptions = new Dictionary<int, string>
			{
				{ 4060, "Cannot open database requested by the login." },
				{ 10928, "Resource limit reached - too many sessions or workloads." },
				{ 10929, "SQL Database has reached maximum workload." },
				{ 10053, "Connection aborted by the host machine." },
				{ 10054, "Connection reset by peer." },
				{ 10060, "Connection timed out." },
				{ 40197, "The service has encountered an error processing your request." },
				{ 40501, "The service is currently busy. Retry the request after 10 seconds." },
				{ 40613, "Database is currently unavailable." }
			};

			return sqlException.Errors
				.Cast<SqlError>()
				.Select(error => new
				{
					ErrorCode = error.Number,
					Description = transientErrorDescriptions.ContainsKey(error.Number)
					? transientErrorDescriptions[error.Number]
					: error.Message
				})
				.Cast<object>()
				.ToList();
		}

		return null;
	}
}