namespace Allen.API;

public class UpdateTagModelValidator : AbstractValidator<UpdateTagModel>
{
    public UpdateTagModelValidator() 
    {
        RuleFor(x => x.NameTag).NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
