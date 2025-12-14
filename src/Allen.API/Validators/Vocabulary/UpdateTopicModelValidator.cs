namespace Allen.API;

public class UpdateTopicModelValidator : AbstractValidator<UpdateTopicModel>
{
    public UpdateTopicModelValidator()
    {
        RuleFor(x => x.TopicName).NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
