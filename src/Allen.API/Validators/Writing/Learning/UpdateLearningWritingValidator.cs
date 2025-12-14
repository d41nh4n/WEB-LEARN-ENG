namespace Allen.API;

public class UpdateLearningWritingValidator : AbstractValidator<UpdateLearningWritingModel>
{
    public UpdateLearningWritingValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new UpdateLearningUnitForWritingModelValidator()!);

        RuleFor(x => x.ContentEN)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.ContentVN)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}