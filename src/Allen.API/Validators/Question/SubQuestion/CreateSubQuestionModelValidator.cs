namespace Allen.API;

public class CreateSubQuestionModelValidator : AbstractValidator<CreateSubQuestionModel>
{
    public CreateSubQuestionModelValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Prompt)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        // Nếu có Options thì không được rỗng
        When(x => x.Options != null, () =>
        {
            RuleFor(x => x.Options)
                .Must(opts => opts != null && opts.Count > 0)
                .WithMessage(ErrorMessageBase.ListNotEmpty);
        });

        // Nếu có CorrectAnswer thì không được trống
        When(x => x.CorrectAnswer != null, () =>
        {
            RuleFor(x => x.CorrectAnswer)
                .NotEmpty()
                .WithMessage("CorrectAnswer cannot be empty if provided.");
        });
    }
}
