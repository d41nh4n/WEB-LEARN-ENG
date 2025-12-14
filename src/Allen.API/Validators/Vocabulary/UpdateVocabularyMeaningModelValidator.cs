namespace Allen.API;

public class UpdateVocabularyMeaningModelValidator : AbstractValidator<UpdateVocabularyMeaningModel>
{
    public UpdateVocabularyMeaningModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ErrorMessageBase.Required);

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

