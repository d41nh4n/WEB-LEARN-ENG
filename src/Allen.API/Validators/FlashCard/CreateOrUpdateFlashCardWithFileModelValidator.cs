namespace Allen.API;

public class CreateOrUpdateFlashCardWithFileModelValidator : AbstractValidator<CreateOrUpdateFlashCardWithFileModel>
{
    public CreateOrUpdateFlashCardWithFileModelValidator()
    {
        // ----------------------------------------------------
        // 1. KIỂM TRA NỘI DUNG TỐI THIỂU (Minimum Content)
        // ----------------------------------------------------

        // Mặt trước: Phải có ít nhất một nội dung (Text HOẶC File).
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.TextFrontCard) || x.FrontImgFile != null || x.FrontAudioFile != null)
            .WithMessage("Flashcard must contain at least one piece of content (text, image, or audio) on the front side.");

        // Mặt sau: Phải có ít nhất một nội dung (Text HOẶC File).
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.TextBackCard) || x.BackImgFile != null || x.BackAudioFile != null)
            .WithMessage("Flashcard must contain at least one piece of content (text, image, or audio) on the back side.");

        // ----------------------------------------------------
        // 2. KIỂM TRA ĐỘ DÀI VĂN BẢN (Text Length)
        // ----------------------------------------------------

        RuleFor(x => x.TextFrontCard)
            .MaximumLength(500).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.TextBackCard)
            .MaximumLength(500).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.Hint)
            .MaximumLength(500).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.PersonalNotes)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);

        // ----------------------------------------------------
        // 3. KIỂM TRA TỆP ĐÍNH KÈM (File Attachments)
        // ----------------------------------------------------

        // Mặt trước: Ảnh
        RuleFor(x => x.FrontImgFile!)
            .SetValidator(new ImageFileValidator<CreateOrUpdateFlashCardWithFileModel>())
            .When(x => x.FrontImgFile != null);

        // Mặt trước: Âm thanh
        RuleFor(x => x.FrontAudioFile!)
            .SetValidator(new AudioFileValidator<CreateOrUpdateFlashCardWithFileModel>())
            .When(x => x.FrontAudioFile != null);

        // Mặt sau: Ảnh
        RuleFor(x => x.BackImgFile!)
            .SetValidator(new ImageFileValidator<CreateOrUpdateFlashCardWithFileModel>())
            .When(x => x.BackImgFile != null);

        // Mặt sau: Âm thanh
        RuleFor(x => x.BackAudioFile!)
            .SetValidator(new AudioFileValidator<CreateOrUpdateFlashCardWithFileModel>())
            .When(x => x.BackAudioFile != null);
    }
}

// ----------------------------------------------------
// --- CUSTOM FILE VALIDATORS (Sử dụng FileValidationConstants) ---
// ----------------------------------------------------

/// <summary>
/// Validator kiểm tra định dạng và kích thước của tệp ảnh (IFormFile).
/// </summary>
public class ImageFileValidator<T> : AbstractValidator<IFormFile>
{
    public ImageFileValidator()
    {
        RuleFor(file => file.Length)
            .LessThanOrEqualTo(AppConstants.MaxBlobSize)
            .WithMessage(AppConstants.FileTooLargeMessage);

        RuleFor(file => file.ContentType)
            .Must(type => AppConstants.AllowedImageMimeTypes.Contains(type.ToLower()))
            .WithMessage(AppConstants.InvalidImageFormatMessage);
    }
}

/// <summary>
/// Validator kiểm tra định dạng và kích thước của tệp âm thanh (IFormFile).
/// </summary>
public class AudioFileValidator<T> : AbstractValidator<IFormFile>
{
    public AudioFileValidator()
    {
        RuleFor(file => file.Length)
            .LessThanOrEqualTo(AppConstants.MaxBlobSize)
            .WithMessage(AppConstants.FileTooLargeMessage);

        RuleFor(file => file.ContentType)
            .Must(type => AppConstants.AllowedAudioMimeTypes.Contains(type.ToLower()))
            .WithMessage(AppConstants.InvalidAudioFormatMessage);
    }
}