namespace Allen.API;

public class SubmitIeltsReadingModelValidator : AbstractValidator<SubmitIeltsModel>
{
    public SubmitIeltsReadingModelValidator()
    {
        RuleFor(x => x.LearningUnitId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleForEach(x => x.Answers)
            .SetValidator(new ReadingAnswerRequestValidator());
    }
}
