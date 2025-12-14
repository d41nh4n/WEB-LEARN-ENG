namespace Allen.API;

/// <summary>
/// Validator cho Query Params khi dùng Cursor Pagination
/// </summary>
public class CursorQueryInfoValidator : AbstractValidator<CursorQueryInfo>
{
    private const int MAX_LIMIT = 100;

    public CursorQueryInfoValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage(ErrorMessageBase.GreaterThan)
            .LessThanOrEqualTo(MAX_LIMIT)
            .WithMessage(ErrorMessageBase.LessThanOrEqual);

        RuleFor(x => x.AfterCursor)
            .GreaterThan(0)
            .When(x => x.AfterCursor.HasValue)
            .WithMessage(ErrorMessageBase.GreaterThan);
    }
}
