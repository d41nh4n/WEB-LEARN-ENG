namespace Allen.API;

public class CreateListeningForIeltsModelValidator : AbstractValidator<CreateListeningForIeltsModel>
{
    public CreateListeningForIeltsModelValidator()
    {
        RuleFor(x => x.LearningUnitId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Media)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateMediaWithoutTranscriptModelValidator());

        RuleFor(x => x.SectionIndex)
            .InclusiveBetween(1, 4)
            .When(x => x.SectionIndex.HasValue)
            .WithMessage("Reading passage number must be between 1 and 4.");

        RuleFor(x => x.EstimatedReadingTime)
            .GreaterThan(0).When(x => x.EstimatedReadingTime.HasValue).WithMessage(ErrorMessageBase.GreaterThan);
    }
}
