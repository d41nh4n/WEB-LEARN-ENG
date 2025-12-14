namespace Allen.API;

public class CreateLearningReadingPassageValidator : AbstractValidator<CreateLearningReadingPassageModel>
{
	public CreateLearningReadingPassageValidator()
	{

		RuleFor(x => x.LearningUnit).SetValidator(new CreateLearningUnitForReadingModelValidator()!);
        RuleForEach(x => x.Paragraphs).SetValidator(new CreateReadingParagraphValidator());
	}
}
