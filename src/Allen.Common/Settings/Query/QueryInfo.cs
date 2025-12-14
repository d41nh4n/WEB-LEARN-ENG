namespace Allen.Common;

public class QueryInfo
{
	public int Top { get; set; } = AppConstants.DefaultPageTop;
	public int Skip { get; set; } = AppConstants.DefaultPageSkip;
	public string? SearchText { get; set; }
	public OrderType OrderType { get; set; } = OrderType.Descending;
	public string? OrderBy { get; set; } = AppConstants.DefaultOrderBy;
	public bool IsDeleted { get; set; } = true;
	public bool NeedTotalCount { get; set; } = AppConstants.DefaultNeedTotalCount;
}
