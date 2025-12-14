namespace Allen.API;

public class CreateIeltsReadingPassagesValidator : AbstractValidator<CreateIeltsReadingPassagesModel>
{
	public CreateIeltsReadingPassagesValidator()
	{
        RuleFor(x => x.Content).NotEmpty().WithMessage(ErrorMessageBase.Required);
		RuleFor(x => x.EstimatedReadingTime)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
		RuleFor(x => x.LearningUnit)
			.NotEmpty().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateLearningUnitForReadingModelValidator()!);
        RuleForEach(x => x.Questions).SetValidator(new CreateQuestionModelValidator());
	}
}
