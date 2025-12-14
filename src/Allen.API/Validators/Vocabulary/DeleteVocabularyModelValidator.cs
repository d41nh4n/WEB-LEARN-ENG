namespace Allen.API;

public class DeleteVocabularyModelValidator : AbstractValidator<DeleteVocabularyModel>
{
    public DeleteVocabularyModelValidator()
    {
        RuleFor(x => x.VocabularyId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}