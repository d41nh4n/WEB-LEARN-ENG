namespace Allen.API;

public class UpdateListeningForIeltsModelValidator : AbstractValidator<UpdateListeningForIeltsModel>
{
    public UpdateListeningForIeltsModelValidator()
    {
        RuleFor(x => x.LearningUnit)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new UpdateLearningUnitForWritingModelValidator());

        RuleForEach(x => x.Data)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .SetValidator(new UpdateListeningSectionModelValidator());
    }
}
