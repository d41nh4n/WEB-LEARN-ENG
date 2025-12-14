namespace Allen.API;

public class IeltsWritingSubmitModelValidator : AbstractValidator<IeltsWritingSubmitModel>
{
	public IeltsWritingSubmitModelValidator()
	{
		RuleFor(x => x.WritingId)
			.NotEmpty()
			.WithMessage(ErrorMessageBase.Required);

		RuleFor(x => x.WritingTaskType)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.Must(value => Enum.GetNames(typeof(WritingTaskType)).Contains(value));

		RuleFor(x => x.Content)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
			.MaximumLength(2000).WithMessage(ErrorMessageBase.MaxLength);
	}
}