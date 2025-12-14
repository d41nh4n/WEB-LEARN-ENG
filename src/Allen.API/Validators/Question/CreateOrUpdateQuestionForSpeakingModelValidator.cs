namespace Allen.API;

public class CreateOrUpdateQuestionForSpeakingModelValidator : AbstractValidator<CreateOrUpdateQuestionForSpeakingModel>
{
	public CreateOrUpdateQuestionForSpeakingModelValidator()
	{
		RuleFor(x => x.ModuleItemId)
			.NotEmpty().WithMessage(ErrorMessageBase.Required);

		RuleFor(x => x.QuestionType)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.Must(value => Enum.GetNames(typeof(QuestionType)).Contains(value));

		RuleFor(x => x.Label)
			.NotNull().WithMessage(ErrorMessageBase.Required);

		RuleFor(x => x.Prompt)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.MaximumLength(2000).WithMessage(ErrorMessageBase.MaxLength);

	}
}
