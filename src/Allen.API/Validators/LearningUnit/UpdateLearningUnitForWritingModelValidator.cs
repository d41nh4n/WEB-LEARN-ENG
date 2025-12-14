namespace Allen.API;

public class UpdateLearningUnitForWritingModelValidator : AbstractValidator<UpdateLearningUnitForWritingModel>
{
    public UpdateLearningUnitForWritingModelValidator()
    {
        RuleFor(model => model.Title)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(model => model.Description)
            .MaximumLength(500).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(model => model.Level)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(LevelType)).Contains(value));
    }
}