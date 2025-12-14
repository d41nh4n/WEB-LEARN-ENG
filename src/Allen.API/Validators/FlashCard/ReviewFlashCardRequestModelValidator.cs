namespace Allen.API.Validators.FlashCard
{
    public class ReviewFlashCardRequestModelValidator : AbstractValidator<ReviewFlashCardRequestModel>
    {
        public ReviewFlashCardRequestModelValidator()
        {
            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage(ErrorMessageBase.Required)
                .Must(type => Enum.GetNames(typeof(RatingLearningCard)).Contains(type)).WithMessage(ErrorMessageBase.Invalid)
                .WithMessage(ErrorMessageBase.Invalid);
        }
    }
}