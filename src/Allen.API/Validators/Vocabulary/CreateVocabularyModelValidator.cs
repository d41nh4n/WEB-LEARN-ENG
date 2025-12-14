namespace Allen.API;

public class CreateVocabularyModelValidator: AbstractValidator<CreateVocabularyModel>
{
    public CreateVocabularyModelValidator()
    {
        RuleFor(x => x.Word)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Level)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.TopicId)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.VocabularyMeaningModels)
            .NotNull()
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        RuleForEach(x => x.VocabularyMeaningModels)
            .SetValidator(new CreateVocabularyMeaningModelValidator());
    }
}
