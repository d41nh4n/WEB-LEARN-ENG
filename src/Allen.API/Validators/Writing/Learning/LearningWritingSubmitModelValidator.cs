namespace Allen.API;

public class LearningWritingSubmitModelValidator : AbstractValidator<LearningWritingSubmitModel>
{
    public LearningWritingSubmitModelValidator()
    {
        RuleFor(x => x.WritingId)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(2000).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.SentenceIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessageBase.GreaterThanOrEqual);

        RuleFor(x => x.Mode)
            .IsInEnum()
            .WithMessage(ErrorMessageBase.Invalid);
    }
}
