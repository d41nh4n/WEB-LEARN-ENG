namespace Allen.API;

public class UpdateReadingPassageForLearningModelValidator : AbstractValidator<UpdateReadingPassageForLearningModel>
{
    public UpdateReadingPassageForLearningModelValidator()
    {
        RuleFor(x => x.LearningUnit).SetValidator(new UpdateLearningUnitForWritingModelValidator()!).When(x => x.LearningUnit != null);
        RuleForEach(x => x.Paragraphs).SetValidator(new UpdateReadingParagraphModelValidator());

        RuleFor(x => x.Paragraphs)
               .Must(HaveSequentialOrders)
               .WithMessage("Paragraphs phải có thứ tự liên tiếp bắt đầu từ 0 (ví dụ: 0,1,2,3...) và không được bỏ sót hoặc lộn xộn.");
    }
    private bool HaveSequentialOrders(List<UpdateReadingParagraphModel> paragraphs)
    {
        if (paragraphs == null || paragraphs.Count == 0)
            return true; 

        var sortedOrders = paragraphs.Select(p => p.Order).OrderBy(o => o).ToList();

        for (int i = 0; i < sortedOrders.Count; i++)
        {
            if (sortedOrders[i] != i)
                return false;
        }

        return true;
    }
}
