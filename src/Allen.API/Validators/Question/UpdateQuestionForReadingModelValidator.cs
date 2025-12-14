namespace Allen.API;

public class UpdateQuestionForReadingModelValidator : AbstractValidator<UpdateQuestionModel>
{
    public UpdateQuestionForReadingModelValidator()
    {
        //RuleFor(x => x.Id)
        //    .NotEmpty().WithMessage(ErrorMessageBase.Required);


        RuleFor(x => x.QuestionType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(QuestionType)).Contains(value));

        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);

        When(x => RequiresOptionsAndAnswerTypes.Contains(x.QuestionType), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
        });
    }
    private static readonly string[] RequiresOptionsAndAnswerTypes = new[]
    {
        nameof(QuestionType.MultipleChoice),
        nameof(QuestionType.Ordering),
        nameof(QuestionType.Matching),
        nameof(QuestionType.FillInBlank),
        nameof(QuestionType.ShortAnswer),
        nameof(QuestionType.Listening),
        nameof(QuestionType.Speaking)
    };
}
