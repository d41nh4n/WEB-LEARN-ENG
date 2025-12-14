namespace Allen.API;

public class CreateTopicModelValidator : AbstractValidator<CreateTopicModel>
{
    public CreateTopicModelValidator()
    {
        RuleFor(x => x.TopicName)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
