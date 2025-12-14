namespace Allen.API;
public class CreateFlashCardByVocabularyModelValidator : AbstractValidator<CreateFlashCardByVocabularyModel>
{
    public CreateFlashCardByVocabularyModelValidator()
    {
        RuleFor(x => x.VocabularyIds)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .Must(list => list == null || (list.Count >= 1 && list.Count <= 50))
            .WithMessage("FlashCard must have between 1 and 50 items.");
    }
}
