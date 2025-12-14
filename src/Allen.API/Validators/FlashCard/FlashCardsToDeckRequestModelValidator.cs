namespace Allen.API;

public class FlashCardsToDeckRequestModelValidator : AbstractValidator<FlashCardsToDeckRequestModel>
{
    public FlashCardsToDeckRequestModelValidator()
    {
        RuleFor(x => x.DeckSourceId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
            
        RuleFor(x => x.FlashCardIds)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .Must(list => list == null || (list.Count >= 1 && list.Count <= 50))
            .WithMessage("FlashCard must have between 1 and 50 items.");
    }
}
