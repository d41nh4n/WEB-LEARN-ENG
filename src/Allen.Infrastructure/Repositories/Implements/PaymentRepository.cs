namespace Allen.Infrastructure;

[RegisterService(typeof(IPaymentRepository))]
public class PaymentRepository(SqlApplicationDbContext context) : RepositoryBase<PaymentEntity>(context), IPaymentRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<PaymentEntity?> GetByOrderCodeAsync(long orderCode)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
    }

    public async Task<QueryResult<Payment>> GetAllPaidPaymentsAsync(QueryInfo queryInfo)
    {
        var query = _context.Payments
            .AsNoTracking()
            .Where(p => p.Status == "PAID" &&
                        (queryInfo.SearchText == null ||
                         EF.Functions.Collate(p.Description ?? "", "Latin1_General_CI_AI")
                             .Contains(queryInfo.SearchText)))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new Payment
            {
                PaymentId = p.Id,
                OrderCode = p.OrderCode,
                Amount = p.Amount,
                Description = p.Description,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                User = new UserModels
                {
                    UserId = p.User!.Id,
                    UserName = p.User.Name
                },
                Package = new PackageModels
                {
                    PackageId = p.Package!.Id,
                    PackageName = p.Package.Name
                }
            });

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
        var data = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<Payment>
        {
            Data = data,
            TotalCount = total
        };
    }

    public async Task<QueryResult<Payment>> GetPaidPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        var query = _context.Payments
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.Status == "PAID" &&
                        (queryInfo.SearchText == null ||
                         EF.Functions.Collate(p.Description ?? "", "Latin1_General_CI_AI")
                             .Contains(queryInfo.SearchText)))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new Payment
            {
                PaymentId = p.Id,
                OrderCode = p.OrderCode,
                Amount = p.Amount,
                Description = p.Description,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                User = new UserModels
                {
                    UserId = p.User!.Id,
                    UserName = p.User.Name
                },
                Package = new PackageModels
                {
                    PackageId = p.Package!.Id,
                    PackageName = p.Package.Name
                }
            });

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
        var data = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<Payment>
        {
            Data = data,
            TotalCount = total
        };
    }
}