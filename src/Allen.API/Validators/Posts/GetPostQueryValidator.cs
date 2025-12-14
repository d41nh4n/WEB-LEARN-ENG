namespace Allen.API;

public class GetPostQueryValidator : AbstractValidator<GetPostQuery>
{
    public GetPostQueryValidator()
    {
        RuleFor(x => x.Privacy)
            .Must(value => Enum.GetNames(typeof(PrivacyType)).Contains(value));
    }
}
