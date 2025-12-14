namespace Allen.API;

public class FlashCardContentsModelValidator : AbstractValidator<FlashCardContentsModel>
{
    public FlashCardContentsModelValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(type => Enum.GetNames(typeof(FlashCardDataType)).Contains(type)).WithMessage(ErrorMessageBase.Invalid);

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(500)
            .WithMessage(ErrorMessageBase.MaxLength);
    }
}
