namespace Allen.API;

public class FlashCardCreateRequestModelValidator : AbstractValidator<FlashCardCreateRequestModel>
{
    public FlashCardCreateRequestModelValidator()
    {
        // Hint tối đa 200 ký tự
        RuleFor(model => model.Hint)
            .MaximumLength(200)
            .WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.FrontContents)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .Must(list => list.Count >= 1 && list.Count <= 3)
            .WithMessage("FrontContents must have between 1 and 3 items.");

        // 2) Không được trùng Type
        RuleFor(x => x.FrontContents)
            .Must(list =>
            {
                return list.Select(i => i.Type).Distinct(StringComparer.OrdinalIgnoreCase).Count() == list.Count;
            })
            .WithMessage("FrontContents cannot contain duplicate Types.");

        // 3) Mỗi phần tử phải valid theo validator con
        RuleForEach(x => x.FrontContents)
            .SetValidator(new FlashCardContentsModelValidator());

        // BACK CONTENTS Y HỆT
        RuleFor(x => x.BackContents)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .Must(list => list.Count >= 1 && list.Count <= 3)
            .WithMessage("BackContents must have between 1 and 3 items.");

        RuleFor(x => x.BackContents)
            .Must(list =>
            {
                return list.Select(i => i.Type).Distinct(StringComparer.OrdinalIgnoreCase).Count() == list.Count;
            })
            .WithMessage("BackContents cannot contain duplicate Types.");

        RuleForEach(x => x.BackContents)
            .SetValidator(new FlashCardContentsModelValidator()); ;
    }
}