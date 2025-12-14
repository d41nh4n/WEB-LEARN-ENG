namespace Allen.API;

public class LoginValidator : AbstractValidator<LoginModel>
{
	public LoginValidator()
	{
		RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorMessageBase.Required)
			.EmailAddress().WithMessage(ErrorMessageBase.InvalidEmail);
		RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessageBase.Required);
	}
}
