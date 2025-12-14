namespace Allen.API;

public class CreateVocabularyModelValidator : AbstractValidator<CreateVocabularyModel>
{
    public CreateVocabularyModelValidator()
    {
        RuleFor(x => x.Level)
             .NotEmpty().WithMessage(ErrorMessageBase.Required)
             .Must(value => Enum.GetNames(typeof(LevelType)).Contains(value)).WithMessage(ErrorMessageBase.Invalid);

        RuleFor(x => x.Word)
        .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.TopicId)
        .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Level)
            .NotNull().WithMessage(ErrorMessageBase.Required);

        When(x => x.VocabularyMeaningModels != null && x.VocabularyMeaningModels.Count != 0, () =>
        {
            RuleForEach(x => x.VocabularyMeaningModels).SetValidator(new CreateVocabularyMeaningModelValidator()!);
        });
    }
}
