namespace Allen.API;

public class CreateOrUpdateSpeakingIeltsModelValidator : AbstractValidator<CreateOrUpdateSpeakingIeltsModel>
{
	public CreateOrUpdateSpeakingIeltsModelValidator()
	{
		RuleFor(x => x.LearningUnitId)
			.NotEmpty().WithMessage(ErrorMessageBase.Required);

		RuleFor(x => x.SectionIndex)
			.InclusiveBetween(1, 3)
			.When(x => x.SectionIndex.HasValue)
			.WithMessage("Reading passage number must be between 1 and 3.");
	}
}
