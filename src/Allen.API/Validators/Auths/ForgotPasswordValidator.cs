namespace Allen.API;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordModel>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .EmailAddress().WithMessage(ErrorMessageBase.InvalidEmail);
    }
}
