namespace Allen.Domain;

public class RevenueSummaryModel
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? GroupBy { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageRevenue { get; set; }
    public List<RevenueItem> Items { get; set; } = [];
}

public class RevenueItem
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageAmount { get; set; }
}
