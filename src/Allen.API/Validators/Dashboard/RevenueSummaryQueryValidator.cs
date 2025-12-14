namespace Allen.API;

public class RevenueSummaryQueryValidator : AbstractValidator<RevenueSummaryQuery>
{
    public RevenueSummaryQueryValidator()
    {
        RuleFor(x => x.FromDate)
            .NotEmpty().WithMessage("FromDate is required.")
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage("StartDate must be less than or equal to EndDate.");

        RuleFor(x => x.ToDate)
            .NotEmpty().WithMessage("ToDate is required.")
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("EndDate must be greater than or equal to StartDate.");
    }
}
