namespace Allen.API;

public class CreateVocabularyMeaningModelValidator : AbstractValidator<CreateVocabularyMeaningModel>
{
    public CreateVocabularyMeaningModelValidator()
    {
        RuleFor(x => x.PartOfSpeech)
             .NotEmpty().WithMessage(ErrorMessageBase.Required)
             .Must(value => Enum.GetNames(typeof(PartOfSpeechType)).Contains(value)).WithMessage(ErrorMessageBase.Invalid);

        RuleFor(x => x.Pronunciation)
      .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Audio)
      .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.DefinitionVN)
      .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Example1)
     .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}

