namespace Allen.API
{
    public class CreateTranscriptModelValidator : AbstractValidator<CreateTranscriptModel>
    {
        public CreateTranscriptModelValidator()
        {
            RuleFor(x => x.StartTime)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessageBase.GreaterThanOrEqual);

            RuleFor(x => x.EndTime)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessageBase.GreaterThanOrEqual)
                .GreaterThan(x => x.StartTime)
                .WithMessage(ErrorMessageBase.GreaterThan);

            RuleFor(x => x.ContentEN)
                .NotEmpty()
                .WithMessage(ErrorMessageBase.Required)
                .MaximumLength(1000)
                .WithMessage(ErrorMessageBase.MaxLength);

            RuleFor(x => x.ContentVN)
                .NotEmpty()
                .WithMessage(ErrorMessageBase.Required)
                .MaximumLength(1000)
                .WithMessage(ErrorMessageBase.MaxLength);

            RuleFor(x => x.IPA)
                .MaximumLength(1000)
                .WithMessage(ErrorMessageBase.MaxLength);

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessageBase.GreaterThanOrEqual);
        }
    }
}
