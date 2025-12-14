namespace Allen.API;

public class CreateTagModelValidator : AbstractValidator<CreateTagModel>
{
    public CreateTagModelValidator()
    {
        RuleFor(x => x.NameTag)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
