namespace Allen.API;

public class CreateLearningUnitForWritingModelValidator : AbstractValidator<CreateLearningUnitForWritingModel>
{
    public CreateLearningUnitForWritingModelValidator()
    {
        RuleFor(model => model.Title)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(model => model.Level)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(LevelType)).Contains(value));
    }
}
