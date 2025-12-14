namespace Allen.API;

public class CreateVocabularyMeaningModelValidator: AbstractValidator<CreateVocabularyMeaningModel>
{
    public CreateVocabularyMeaningModelValidator()
    {
        RuleFor(x => x.DefinitionEN)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x)
            .Must(HaveAtLeastOneExample)
            .WithMessage("At least one example is required");
    }

    private static bool HaveAtLeastOneExample(CreateVocabularyMeaningModel model)
    {
        return !string.IsNullOrWhiteSpace(model.Example1)
            || !string.IsNullOrWhiteSpace(model.Example2)
            || !string.IsNullOrWhiteSpace(model.Example3);
    }
}
