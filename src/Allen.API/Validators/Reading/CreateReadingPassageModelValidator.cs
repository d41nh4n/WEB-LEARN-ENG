namespace Allen.API;

public class CreateReadingPassageModelValidator : AbstractValidator<CreateReadingPassageModel>
{
    public CreateReadingPassageModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MinimumLength(5).WithMessage(ErrorMessageBase.MinLength);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MinimumLength(5).WithMessage(ErrorMessageBase.MinLength)
            .MaximumLength(3000).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.EstimatedReadingTime)
            .GreaterThan(0).When(x => x.EstimatedReadingTime.HasValue).WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.LearningUnit)
            .Null().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateLearningUnitForReadingModelValidator()!);

        RuleFor(x => x.Questions)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .ForEach(q => q.SetValidator(new CreateOrUpdateQuestionForReadingModelValidator()));
    }
}
