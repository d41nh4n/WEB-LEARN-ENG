namespace Allen.API;

public class SubmitSpeakingIeltsModelValidator : AbstractValidator<SubmitSpeakingIeltsModel>
{
	public SubmitSpeakingIeltsModelValidator()
	{
		RuleFor(x => x.AudioFile)
			.NotNull().WithMessage(ErrorMessageBase.Required)
			.Must(file => file != null && (file.ContentType == "audio/wav" || file.ContentType == "audio/mpeg" || file.ContentType == "audio/mp3"))
			.WithMessage("Audio file must be in WAV or MP3 format.");
		RuleFor(x => x.ReferenceText)
			.NotEmpty().WithMessage(ErrorMessageBase.Required);
	}
}
