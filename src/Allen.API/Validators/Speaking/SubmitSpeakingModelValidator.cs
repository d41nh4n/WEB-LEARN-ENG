namespace Allen.API;

public class SubmitSpeakingModelValidator : AbstractValidator<SubmitSpeakingModel>
{
    public SubmitSpeakingModelValidator()
    {
        RuleFor(x => x.TranscriptId)
            .NotNull().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
