namespace Allen.API;

public class CreateSubQuestionForListeningIeltsModelValidator : AbstractValidator<CreateSubQuestionForListeningIeltsModel>
{
    public CreateSubQuestionForListeningIeltsModelValidator()
    {
		//RuleFor(x => x.Prompt)
		//    .NotEmpty().WithMessage(ErrorMessageBase.Required);

		//RuleFor(x => x.CorrectAnswer)
		//    .NotEmpty().WithMessage(ErrorMessageBase.Required);

		RuleFor(x => x)
			.Must(x =>
				!(x.StartTextIndex.HasValue && x.EndTextIndex.HasValue)
				|| x.StartTextIndex <= x.EndTextIndex
			)
			.WithMessage("StartTextIndex cannot be greater than EndTextIndex.");

	}
}
