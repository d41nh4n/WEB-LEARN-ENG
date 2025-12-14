namespace Allen.API;

public class UpdateMediaModelValidator : AbstractValidator<UpdateMediaModel>
{
    public UpdateMediaModelValidator()
    {
        RuleFor(x => x.MediaType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(type => Enum.GetNames(typeof(MediaType)).Contains(type));

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(256).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.SourceUrl)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(256).WithMessage(ErrorMessageBase.MaxLength)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("SourceUrl must be a valid URL");

        RuleFor(x => x.Transcripts)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .ForEach(q => q.SetValidator(new UpdateTranscriptModelValidator()));
    }
}
