namespace Allen.API;

public class UnSubcriptionModelValidator : AbstractValidator<UnSubcriptionModel>
{
    public UnSubcriptionModelValidator()
    {
        RuleFor(x => x.Endpoint)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
