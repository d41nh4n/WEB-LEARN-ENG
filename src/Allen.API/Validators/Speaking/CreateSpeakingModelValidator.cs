namespace Allen.API;

public class CreateSpeakingModelValidator : AbstractValidator<CreateSpeakingModel>
{
    public CreateSpeakingModelValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateLearningUnitForSpeakingModelValidator()!);

        RuleFor(x => x.Media)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateMediaModelValidator()!);
    }
}
