namespace Allen.API;

public class CreatePostModelValidator : AbstractValidator<CreatePostModel>
{
    public CreatePostModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.Privacy)
            .Must(value => Enum.GetNames(typeof(PrivacyType)).Contains(value));

        When(x => x.Images != null && x.Images.Count > 0, () =>
        {
            RuleFor(x => x.Images!.Count)
                .LessThanOrEqualTo(10).WithMessage("Too many images (max 10).");

            RuleForEach(x => x.Images!)
                .Must(f => new[] { "image/jpeg", "image/png", "image/gif", "image/webp" }.Contains(f.ContentType))
                .WithMessage("Invalid image type.")
                .Must(f => f.Length <= 5 * 1024 * 1024)
                .WithMessage("Image too large (max 5MB).");
        });
    }
}

