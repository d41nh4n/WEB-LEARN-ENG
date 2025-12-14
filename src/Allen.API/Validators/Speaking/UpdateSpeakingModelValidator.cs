namespace Allen.API;

public class UpdateSpeakingModelValidator : AbstractValidator<UpdateSpeakingModel>
{
    public UpdateSpeakingModelValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new UpdateLearningUnitForSpeakingModelValidator()!);

        RuleFor(x => x.Media)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new UpdateMediaModelValidator()!);
    }
}