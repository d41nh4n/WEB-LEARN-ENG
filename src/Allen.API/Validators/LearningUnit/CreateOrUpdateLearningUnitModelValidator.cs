namespace Allen.API;

public class CreateOrUpdateLearningUnitModelValidator : AbstractValidator<CreateOrUpdateLearningUnitModel>
{
    public CreateOrUpdateLearningUnitModelValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

		RuleFor(model => model.Title)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(model => model.Level)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(LevelType)).Contains(value));

        RuleFor(model => model.SkillType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(SkillType)).Contains(value));

		RuleFor(model => model.LearningUnitType)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.Must(value => Enum.GetNames(typeof(LearningUnitType)).Contains(value));
	}
}
