namespace Allen.API;

public class CreateIeltsWritingModelValidator : AbstractValidator<CreateIeltsWritingModel>
{
    public CreateIeltsWritingModelValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateLearningUnitForWritingModelValidator()!);

        RuleFor(x => x.TaskType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(WritingTaskType)).Contains(value))
            .WithMessage("Invalid TaskType. Must be 'Task1' or 'Task2'.");

        RuleFor(x => x.ContentEN)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);

        When(x => x.SourceUrl != null, () =>
        {
            RuleFor(x => x.SourceUrl!.ContentType)
                .Must(type => new[] { "image/jpeg", "image/png", "image/webp" }.Contains(type))
                .WithMessage("Invalid image type (must be .jpg, .png, or .webp).");

            RuleFor(x => x.SourceUrl!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("Image too large (max 5MB).");
        });
    }
}
