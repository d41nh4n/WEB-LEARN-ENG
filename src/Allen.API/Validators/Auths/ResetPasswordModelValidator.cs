namespace Allen.API;

public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
{
    public ResetPasswordModelValidator()
    {
        RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(ErrorMessageBase.Required)
                .Equal(x => x.NewPassword)
                .WithMessage(ErrorMessageBase.Format(
                    ErrorMessageBase.MustMatch, nameof(ResetPasswordModel.ConfirmPassword), nameof(ResetPasswordModel.NewPassword)));
    }
}
