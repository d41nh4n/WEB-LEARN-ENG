namespace Allen.API;

public class CreateMediaWithoutTranscriptModelValidator : AbstractValidator<CreateMediaWithoutTranscriptModel>
{
    public CreateMediaWithoutTranscriptModelValidator()
    {
        RuleFor(x => x.MediaType)
           .NotEmpty().WithMessage(ErrorMessageBase.Required)
           .Must(type => Enum.GetNames(typeof(MediaType)).Contains(type));

        RuleFor(x => x.SourceUrl)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(256).WithMessage(ErrorMessageBase.MaxLength)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("SourceUrl must be a valid URL");
    }
}
