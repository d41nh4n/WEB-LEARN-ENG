namespace Allen.API;

public class ImportCardsFromDeckModelValidator : AbstractValidator<ImportCardsFromDeckRequestModel>
{
    public ImportCardsFromDeckModelValidator()
    {
        RuleFor(x => x.SourceDeckId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
