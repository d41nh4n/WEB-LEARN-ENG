namespace Allen.API;

public class UpdateListeningForLearningModelValidator : AbstractValidator<UpdateListeningForLearningModel>
{
    public UpdateListeningForLearningModelValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new UpdateLearningForListeningModelValidator());
        RuleFor(x => x.Media)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new UpdateMediaModelValidator());
    }
}
