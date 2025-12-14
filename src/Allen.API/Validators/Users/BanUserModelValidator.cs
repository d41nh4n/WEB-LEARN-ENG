namespace Allen;

public class BanUserModelValidator : AbstractValidator<BanUserModel>
{
    public BanUserModelValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.IsDeleted)
                .Must(value => value == true || value == false)
                .WithMessage("IsDeleted must be either true or false.");
    }
}
