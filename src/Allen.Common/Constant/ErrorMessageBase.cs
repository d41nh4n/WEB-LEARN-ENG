namespace Allen.Common;

public static class ErrorMessageBase
{
    public const string InsufficientPoints = "Insufficient points balance. (Your points: {0}, required: {1})";

    public const string Required = "{PropertyName} is required";
	public const string Invalid = "{PropertyName} is invalid";
	public const string InvalidEmail = "Invalid email format";
	public const string InvalidPhoneNumber = "{PropertyName} is not a valid phone number";
	public const string InvalidDate = "{PropertyName} is not a valid date";

	public const string MinLength = "{PropertyName} must be at least {MinLength} characters";
	public const string MaxLength = "{PropertyName} must not exceed {MaxLength} characters";
	public const string ExactLength = "{PropertyName} must be exactly {ExactLength} characters";

	public const string GreaterThan = "{PropertyName} must be greater than {ComparisonValue}";
	public const string GreaterThanOrEqual = "{PropertyName} must be greater than or equal to {ComparisonValue}";
	public const string LessThan = "{PropertyName} must be less than {ComparisonValue}";
	public const string LessThanOrEqual = "{PropertyName} must be less than or equal to {ComparisonValue}";
	public const string Range = "{PropertyName} must be between {MinLength} and {MaxLength} characters";
    public const string Between = "{PropertyName} must be between {MinLength} and {MaxLength} items";

    public const string OnlyLetters = "{PropertyName} can only contain letters";
	public const string OnlyNumbers = "{PropertyName} can only contain numbers";
	public const string OnlyAlphanumeric = "{PropertyName} can only contain letters and numbers";
	public const string InvalidFormat = "{PropertyName} has an invalid format";

	public const string ListNotEmpty = "{PropertyName} must not be empty";
	public const string ListMinItems = "{PropertyName} must contain at least {MinItems} items";
	public const string ListMaxItems = "{PropertyName} must contain no more than {MaxItems} items";

	public const string MustBeTrue = "{PropertyName} must be true";
	public const string MustBeFalse = "{PropertyName} must be false";
	public const string MustNull = "{PropertyName} must be null";

	public const string NotContainSpaces = "{PropertyName} must not contain spaces";
    public const string ProhibitedContent = "Your comment contains prohibited words";

    public const string MustMatch = "{PropertyName} must match {ComparisonProperty}";
	public const string MustNotMatch = "{PropertyName} must not be the same as {ComparisonProperty}";
	public const string AlreadyExists = "{0} with value '{1}' already exists";
    public const string NotExists = "{0} with value '{1}' not exists";


    public static string NotFound = "{0} with ID {1} was not found";
	public static string BadRequest = "";
	public static string Forbidden = "You do not have the required permissions";

	public const string CreatedSuccess = "Created {0} successfully";
	public const string UpdatedSuccess = "Updated {0} successfully";
	public const string DeletedSuccess = "Deleted {0} successfully";
    public const string RetrievedSuccess = "Retrieved {0} successfully";

    public const string GetFailure = "Failed to get {0}";
    public const string CreateFailure = "Failed to create {0}";
	public const string UpdateFailure = "Failed to update {0}";
	public const string DeleteFailure = "Failed to delete {0}";

    public const string FileTooLargeMessage = "File size must not exceed 5 MB.";
    public const string InvalidImageFormatMessage = "Only JPEG, PNG formats are allowed.";
    public const string InvalidAudioFormatMessage = "Only MP3 audio formats are allowed.";

    public const string Existed = "{0} with value '{1}' already exists";
	public static string Format(string message, params object[] values)
		=> string.Format(message, values);
}
