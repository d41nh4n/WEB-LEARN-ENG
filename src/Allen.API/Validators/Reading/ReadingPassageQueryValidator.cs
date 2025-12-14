namespace Allen.API;

public class ReadingPassageQueryValidator : AbstractValidator<ReadingPassageQuery>
{
    public ReadingPassageQueryValidator()
    {
        RuleFor(x => x.ReadingPassageNumber)
                .Must(list => list == null || list.Count == 0 || list.All(n => new[] { 1, 2, 3 }.Contains(n)))
                .WithMessage("ReadingPassageNumber must only contain values 1, 2, or 3.");
    }
}
