using Allen.Common.Settings.Enum;

namespace Allen.API;

public class UpdateDeckModelValidator : AbstractValidator<UpdateDeckModel>
{
    public UpdateDeckModelValidator()
    {
        // Nếu có DeckName thì kiểm tra độ dài
        RuleFor(model => model.DeckName)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100)
            .WithMessage(ErrorMessageBase.MaxLength);

        // Nếu có Description thì kiểm tra độ dài
        RuleFor(model => model.Description)
            .MaximumLength(500)
            .WithMessage(ErrorMessageBase.MaxLength);

        // Nếu có Level thì phải là enum hợp lệ
        RuleFor(model => model.Level)
           .Must(value => Enum.GetNames(typeof(DeckLevel)).Contains(value)).WithMessage(ErrorMessageBase.Invalid);
    }
}
