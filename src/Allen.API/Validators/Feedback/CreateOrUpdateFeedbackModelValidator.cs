namespace Allen.API;

public class CreateOrUpdateFeedbackModelValidator : AbstractValidator<CreateOrUpdateFeedbackModel>
{
	public CreateOrUpdateFeedbackModelValidator()
	{
		RuleFor(x => x.Title)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.MaximumLength(50).WithMessage(ErrorMessageBase.MaxLength);

		RuleFor(x => x.Description)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);

		RuleFor(x => x.Image)
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
