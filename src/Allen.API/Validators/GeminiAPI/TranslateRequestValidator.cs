namespace Allen.API;

public class TranslateRequestValidator : AbstractValidator<TranslateRequest>
{
    public TranslateRequestValidator()
    {
        RuleFor(x => x.Prompt)
            .MaximumLength(50).WithMessage(ErrorMessageBase.MaxLength);
    }
}