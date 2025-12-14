namespace Allen.API;

public class ReadingAnswerRequestValidator : AbstractValidator<AnswerRequest>
{
    public ReadingAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
        //RuleFor(x => x.SubQuestionId)
        //    .NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
