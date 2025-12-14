namespace Allen.API;

public class LearningUnitQueryValidator : AbstractValidator<LearningUnitQuery>
{
    public LearningUnitQueryValidator()
    {
        RuleFor(x => x.SkillType)
            .Must(value => string.IsNullOrEmpty(value) || Enum.GetNames(typeof(SkillType)).Contains(value));

        RuleFor(x => x.LevelType)
            .Must(value => string.IsNullOrEmpty(value) || Enum.GetNames(typeof(LevelType)).Contains(value));

        RuleFor(x => x.LearningUnitType)
            .Must(value => string.IsNullOrEmpty(value) || Enum.GetNames(typeof(LearningUnitType)).Contains(value));

		RuleFor(x => x.LearningUnitStatusType)
            .NotNull().WithMessage(ErrorMessageBase.Required)
			.Must(value => Enum.GetNames(typeof(LearningUnitStatusType)).Contains(value));

        RuleFor(x => x.TaskType)
            .Must(value => string.IsNullOrEmpty(value) || Enum.GetNames(typeof(WritingTaskType)).Contains(value));
	}
}
