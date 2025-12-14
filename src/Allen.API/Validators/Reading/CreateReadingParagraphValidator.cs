namespace Allen.API;

public class CreateReadingParagraphValidator : AbstractValidator<CreateReadingParagraphModel>
{
	public CreateReadingParagraphValidator()
	{
		//RuleFor(p => p.Order).GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
		RuleFor(p => p.OriginalText).NotEmpty().WithMessage(ErrorMessageBase.Required);
		RuleFor(p => p.Transcript).NotEmpty().WithMessage(ErrorMessageBase.Required);
	}
}
