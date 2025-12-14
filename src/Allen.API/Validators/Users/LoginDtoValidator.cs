namespace Allen.API;

public class LoginDtoValidator : AbstractValidator<User>
{
	public LoginDtoValidator()
	{
		RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorMessageBase.Required)
			.EmailAddress().WithMessage(ErrorMessageBase.InvalidEmail);
		RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessageBase.Required);
	}
}