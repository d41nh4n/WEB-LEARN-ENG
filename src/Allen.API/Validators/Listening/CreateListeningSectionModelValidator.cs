namespace Allen.API;

public class CreateListeningSectionModelValidator : AbstractValidator<CreateListeningSectionModel>
{
    public CreateListeningSectionModelValidator()
    {
        RuleFor(x => x.EstimatedReadingTime).GreaterThan(0);
        RuleFor(x => x.Media).NotNull().SetValidator(new CreateMediaWithoutTranscriptModelValidator());
        RuleForEach(x => x.Questions).NotNull().SetValidator(new CreateQuestionForListeningIeltsModelValidator());
    }
}
