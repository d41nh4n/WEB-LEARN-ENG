namespace Allen.API;

public class CreateOrUpdateUnitStepModelValidator : AbstractValidator<CreateOrUpdateUnitStepModel>
{
    public CreateOrUpdateUnitStepModelValidator()
    {
        RuleFor(x => x.LearningUnitId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
