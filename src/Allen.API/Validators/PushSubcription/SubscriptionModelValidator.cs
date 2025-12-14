namespace Allen.API;

public class SubscriptionModelValidator : AbstractValidator<SubscriptionModel>
{
    public SubscriptionModelValidator()
    {
        RuleFor(x => x.Endpoint)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.P256dh)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Auth)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
