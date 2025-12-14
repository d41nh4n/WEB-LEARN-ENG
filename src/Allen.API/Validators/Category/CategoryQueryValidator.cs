namespace Allen.API;

public class CategoryQueryValidator : AbstractValidator<CategoryQuery>
{
    public CategoryQueryValidator()
    {
        RuleFor(x => x.SkillType)
            .Must(value => Enum.TryParse<SkillType>(value, out _))
            .WithMessage(ErrorMessageBase.Invalid)
            .When(x => x.SkillType != null);

        RuleFor(x => x.QueryInfo).SetValidator(new QueryInfoValidator());
    }
}
