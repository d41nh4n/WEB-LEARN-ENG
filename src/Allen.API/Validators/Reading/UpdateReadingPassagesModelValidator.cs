namespace Allen.API;

public class UpdateReadingPassagesModelValidator : AbstractValidator<UpdateReadingPassagesModel>
{
    public UpdateReadingPassagesModelValidator()
    {
        RuleFor(x => x.Title)
        .NotEmpty().WithMessage(ErrorMessageBase.Required)
        .MinimumLength(5).WithMessage(ErrorMessageBase.MinLength);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MinimumLength(5).WithMessage(ErrorMessageBase.MinLength);

        RuleFor(x => x.EstimatedReadingTime)
            .GreaterThan(0).When(x => x.EstimatedReadingTime.HasValue).WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.ReadingPassageNumber)
            .InclusiveBetween(1, 3)
            .When(x => x.ReadingPassageNumber.HasValue)
            .WithMessage("Reading passage number must be between 1 and 3.");
    }
}
