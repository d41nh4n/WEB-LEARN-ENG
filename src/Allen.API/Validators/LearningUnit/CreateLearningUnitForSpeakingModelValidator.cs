namespace Allen.API;

public class CreateLearningUnitForSpeakingModelValidator : AbstractValidator<CreateLearningUnitForSpeakingModel>
{
    public CreateLearningUnitForSpeakingModelValidator()
    {
        RuleFor(model => model.Title)
        .NotEmpty().WithMessage(ErrorMessageBase.Required)
        .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(model => model.Level)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(LevelType)).Contains(value));

        RuleFor(model => model.SkillType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(SkillTypeForSpeaking)).Contains(value));
    }
}
