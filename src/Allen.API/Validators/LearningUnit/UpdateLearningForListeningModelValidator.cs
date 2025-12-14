namespace Allen.API;

public class UpdateLearningForListeningModelValidator : AbstractValidator<UpdateLearningForListeningModel>
{
    public UpdateLearningForListeningModelValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(200).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(x => x.Level)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(LevelType)).Contains(value));
    }
}
