namespace Allen.API;

public class FlashCardUpdateRequestModelValidator : AbstractValidator<FlashCardUpdateRequestModel>
{
    public FlashCardUpdateRequestModelValidator()
    {
        // Nếu có FrontContents thì phải hợp lệ
        RuleFor(x => x.FrontContents)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .Must(list => list == null || (list.Count >= 1 && list.Count <= 3))
            .WithMessage("FrontContents must have between 1 and 3 items.");

        RuleFor(x => x.FrontContents)
            .Must(list =>
            {
                if (list == null) return true;
                return list.Select(i => i.Type).Distinct(StringComparer.OrdinalIgnoreCase).Count() == list.Count;
            })
            .WithMessage("FrontContents cannot contain duplicate Types.");

        RuleForEach(x => x.FrontContents)
            .SetValidator(new FlashCardContentsModelValidator());

        // Nếu có BackContents thì phải hợp lệ
        RuleFor(x => x.BackContents)
            .NotEmpty().WithMessage(ErrorMessageBase.ListNotEmpty)
            .Must(list => list == null || (list.Count >= 1 && list.Count <= 3))
            .WithMessage("BackContents must have between 1 and 3 items.");

        RuleFor(x => x.BackContents)
            .Must(list =>
            {
                if (list == null) return true;
                return list.Select(i => i.Type).Distinct(StringComparer.OrdinalIgnoreCase).Count() == list.Count;
            })
            .WithMessage("BackContents cannot contain duplicate Types.");

        RuleForEach(x => x.BackContents)
            .SetValidator(new FlashCardContentsModelValidator());

        // Hint tối đa 200 ký tự
        RuleFor(model => model.Hint)
            .MaximumLength(200)
            .WithMessage(ErrorMessageBase.MaxLength);

        // PersonalNotes tối đa 1000 ký tự
        RuleFor(model => model.PersonalNotes)
            .MaximumLength(1000)
            .WithMessage(ErrorMessageBase.MaxLength);
    }
}
