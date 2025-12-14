namespace Allen.API.Validators.Comment
{
    public class GetCommentQueryValidator : AbstractValidator<GetCommentQuery>
    {
        public GetCommentQueryValidator()
        {
            RuleFor(x => x.ObjectId)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
        }
    }
}
