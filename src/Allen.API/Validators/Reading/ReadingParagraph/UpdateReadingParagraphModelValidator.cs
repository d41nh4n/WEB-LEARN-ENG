namespace Allen.API;

public class UpdateReadingParagraphModelValidator : AbstractValidator<UpdateReadingParagraphModel>
{
    public UpdateReadingParagraphModelValidator()
    {
        RuleFor(x => x.Id)
             .Must(id => id == null || id != Guid.Empty).WithMessage(ErrorMessageBase.Invalid);
        //RuleFor(x => x.Order)
        //    .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.OriginalText)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(5000).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(x => x.Transcript)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(5000).WithMessage(ErrorMessageBase.MaxLength);
    }
}
