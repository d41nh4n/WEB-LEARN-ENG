namespace Allen.API;

public class CreateOrUpdateTopicValidator : AbstractValidator<CreateOrUpdateTopicModel>
{
	public CreateOrUpdateTopicValidator()
	{
		RuleFor(RuleFor => RuleFor.TopicName)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);

		RuleFor(RuleFor => RuleFor.TopicDecription)
			.MaximumLength(256).When(RuleFor => !string.IsNullOrEmpty(RuleFor.TopicDecription))
			.WithMessage(ErrorMessageBase.MaxLength);

		RuleFor(RuleFor => RuleFor.SkillType)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.Must(value => Enum.GetNames(typeof(SkillType)).Contains(value));
	}
}
