namespace Allen.API;

public class CreateLearningWritingValidator : AbstractValidator<CreateLearningWritingModel>
{
    public CreateLearningWritingValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateLearningUnitForWritingModelValidator()!);

        RuleFor(x => x.ContentEN)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.ContentVN)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}