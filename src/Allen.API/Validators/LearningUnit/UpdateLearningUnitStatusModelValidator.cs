namespace Allen.API;

public class UpdateLearningUnitStatusModelValidator : AbstractValidator<UpdateLearningUnitStatusModel>
{
	public UpdateLearningUnitStatusModelValidator()
	{
		RuleFor(x => x.LearningUnitStatusType)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.Must(value => Enum.GetNames(typeof(LearningUnitStatusType)).Contains(value));
	}
}
