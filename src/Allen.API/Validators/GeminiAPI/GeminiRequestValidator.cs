namespace Allen.API;

public class GeminiRequestValidator : AbstractValidator<GeminiRequest>
{
    public GeminiRequestValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}