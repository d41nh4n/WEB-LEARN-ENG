namespace Allen.API;

public class CreateListeningForLearningModelValidator : AbstractValidator<CreateListeningForLearningModel>
{
    public CreateListeningForLearningModelValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateLearningUnitForReadingModelValidator());
        RuleFor(x => x.Media)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new CreateMediaModelValidator());
    }
}
