namespace Allen.API;

public class DeleteFlashCardLstModelValidator : AbstractValidator<DeleteFlashCardLstModel>
{
    public DeleteFlashCardLstModelValidator()
    {
        RuleFor(x => x.FlashCardIds)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("FlashCardIds list contains invalid GUIDs.");

        RuleFor(x => x.FlashCardIds)
         .Must(list => list == null || (list.Count >= 1 && list.Count <= 50))
            .WithMessage("FlashCard must have between 1 and 50 items.");
    }
}
