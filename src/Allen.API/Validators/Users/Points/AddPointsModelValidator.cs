namespace Allen.API;

public class AddPointsModelValidator : AbstractValidator<AddPointsModel>
{
    public AddPointsModelValidator()
    {
        RuleFor(x => x.Points)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.Description)
            .MaximumLength(50).WithMessage(ErrorMessageBase.MaxLength);
    }
}