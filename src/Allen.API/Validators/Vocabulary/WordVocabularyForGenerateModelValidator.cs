namespace Allen.API;

public class WordVocabularyForGenerateModelValidator : AbstractValidator<WordVocabularyForGenerateModel>
{
    public WordVocabularyForGenerateModelValidator()
    {
        RuleFor(x => x.Words)
            .NotNull()
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .Must(words => words.Count <= 10)
            .WithMessage(ErrorMessageBase.ListMaxItems);

        RuleForEach(x => x.Words)
            .NotEmpty()
            .WithMessage("Word cannot be empty or whitespace");
    }
}
