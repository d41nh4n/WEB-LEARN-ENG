namespace Allen.API;

public class QuizVocabulariesRequestModelValidator : AbstractValidator<QuizVocabulariesRequestModel>
{
    public QuizVocabulariesRequestModelValidator()
    {
        RuleFor(x => x.NumberA1Words)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThan);
        RuleFor(x => x.NumberA2Words)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThan);
        RuleFor(x => x.NumberB1Words)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThan);
        RuleFor(x => x.NumberB2Words)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThan);
        RuleFor(x => x.NumberC1Words)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThan);
        RuleFor(x => x.NumberC2Words)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThan);
        RuleFor(x => x)
            .Must(x => x.NumberA1Words + x.NumberA2Words + x.NumberB1Words + x.NumberB2Words + x.NumberC1Words + x.NumberC2Words > 0 
            && x.NumberA1Words + x.NumberA2Words + x.NumberB1Words + x.NumberB2Words + x.NumberC1Words + x.NumberC2Words <= 30)
            .WithMessage("At least 1 word must be and less than 30 words requested for the quiz");
    }
}
