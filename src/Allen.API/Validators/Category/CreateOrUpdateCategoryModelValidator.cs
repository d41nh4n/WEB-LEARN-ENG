namespace Allen.API;

public class CreateOrUpdateCategoryModelValidator : AbstractValidator<CreateOrUpdateCategoryModel>
{
    public CreateOrUpdateCategoryModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(x => x.SkillType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.TryParse<SkillType>(value, out _)).WithMessage(ErrorMessageBase.Invalid);
    }
}
