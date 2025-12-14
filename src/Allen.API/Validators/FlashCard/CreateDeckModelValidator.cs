using Allen.Common.Settings.Enum;

namespace Allen.API;

public class CreateDeckModelValidator : AbstractValidator<CreateDeckModel>
{
    public CreateDeckModelValidator()
    {
        // DeckName bắt buộc, tối đa 100 ký tự
        RuleFor(model => model.DeckName)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100)
            .WithMessage(ErrorMessageBase.MaxLength);

        // Description tối đa 500 ký tự (nếu có)
        RuleFor(model => model.Description)
            .MaximumLength(500)
            .WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(model => model.Level)
           .Must(value => Enum.GetNames(typeof(DeckLevel)).Contains(value)).WithMessage(ErrorMessageBase.Invalid);
    }
}