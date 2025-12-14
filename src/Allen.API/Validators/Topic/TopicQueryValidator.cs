namespace Allen.API;

public class TopicQueryValidator : AbstractValidator<TopicQuery>	
{
	public TopicQueryValidator()
	{
		RuleFor(RuleFor => RuleFor.SkillType)
			.Must(value => Enum.GetNames(typeof(SkillType)).Contains(value)).When(x => x.SkillType != null)
			.WithMessage(ErrorMessageBase.InvalidFormat);
	}
}
