namespace Allen.API;

public class CreateCommentModelValidator : AbstractValidator<CreateCommentModel>
{
    public CreateCommentModelValidator()
    {
        RuleFor(x => x.Content)
           .NotEmpty().WithMessage(ErrorMessageBase.Required)
           .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength)
           .Must(content => !ProfanityFilterHelper.ContainsProhibitedWords(content))
           .WithMessage(ErrorMessageBase.ProhibitedContent);

        RuleFor(x => x.ObjectId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
