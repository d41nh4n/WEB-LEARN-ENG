namespace Allen.API.Validators.Comment
{
    public class UpdateCommentModelValidator : AbstractValidator<UpdateCommentModel>
    {
        public UpdateCommentModelValidator()
        {
            RuleFor(x => x.Content)
               .NotEmpty().WithMessage(ErrorMessageBase.Required)
               .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength)
               .Must(content => !ProfanityFilterHelper.ContainsProhibitedWords(content))
               .WithMessage(ErrorMessageBase.ProhibitedContent);
        }
    }
}
