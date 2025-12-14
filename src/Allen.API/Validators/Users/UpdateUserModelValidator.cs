namespace Allen.API;

public class UpdateUserModelValidator : AbstractValidator<UpdateUserModel>
{
	public UpdateUserModelValidator()
	{
		//RuleFor(x => x.Name)
		//	.NotEmpty().WithMessage(ErrorMessageBase.Required)
		//	.MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);
		RuleFor(x => x.Picture)
			.Must(BeAValidImage).WithMessage("Picture must be a valid image file.");
	}
	private bool BeAValidImage(IFormFile? file)
	{
		if (file == null || file.Length == 0)
		{
			return true; // Optional field, so allow null or empty
		}
		var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
		return validImageTypes.Contains(file.ContentType);
	}
}
