namespace Allen.API;

public class FlashCardCreateMultiRequestModelValidator : AbstractValidator<FlashCardCreateMultiRequestModel>
{
    private const int MAX_CARDS_LIMIT = 30;
    public FlashCardCreateMultiRequestModelValidator(IValidator<FlashCardCreateRequestModel> singleCardValidator)
    {
        RuleFor(x => x.Cards)
                .NotEmpty()
                .WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Cards.Count)
                .LessThanOrEqualTo(MAX_CARDS_LIMIT)
                .WithMessage(ErrorMessageBase.LessThanOrEqual);

        RuleForEach(x => x.Cards)
            .SetValidator(singleCardValidator);
    }
}
