namespace Allen.API;

public class CreateOrUpdateReactionModelValidator : AbstractValidator<CreateOrUpdateReactionModel>
{
    public CreateOrUpdateReactionModelValidator()
    {
        RuleFor(x => x.ObjectId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.ReactionType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(ReactionType)).Contains(value));
    }
}
