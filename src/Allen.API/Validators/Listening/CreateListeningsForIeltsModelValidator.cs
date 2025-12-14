namespace Allen.API;

public class CreateListeningsForIeltsModelValidator : AbstractValidator<CreateListeningsForIeltsModel>
{
    public CreateListeningsForIeltsModelValidator()
    {
        RuleFor(x => x.LearningUnit).NotNull().SetValidator(new CreateLearningUnitForWritingModelValidator());
        RuleForEach(x => x.Data).NotNull().SetValidator(new CreateListeningSectionModelValidator());
    }
}
