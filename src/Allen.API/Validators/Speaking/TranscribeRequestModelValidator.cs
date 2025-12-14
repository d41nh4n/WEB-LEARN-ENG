using NAudio.Wave;

namespace Allen.API;

public class TranscribeRequestModelValidator : AbstractValidator<TranscribeRequestModel>
{
    public TranscribeRequestModelValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .Must(file => file != null && file.Length > 0)
                .WithMessage("File file cannot be empty")
            .Must(file => file != null &&
                          file.FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only MP3 files are allowed")
            .Must(file =>
            {
                if (file == null) return false;

                try
                {
                    using (var stream = file.OpenReadStream())
                    using (var reader = new Mp3FileReader(stream))
                    {
                        return reader.TotalTime.TotalSeconds <= 60;
                    }
                }
                catch
                {
                    return false;
                }
            })
            .WithMessage("Audio file must be shorter than 60 seconds");
    }
}
