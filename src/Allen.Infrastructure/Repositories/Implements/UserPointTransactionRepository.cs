namespace Allen.Infrastructure;

[RegisterService(typeof(IUserPointTransactionRepository))]
public class UserPointTransactionRepository(SqlApplicationDbContext context) : RepositoryBase<UserPointTransactionEntity>(context), IUserPointTransactionRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<UserPointTransaction>> GetAllTransactionsAsync(QueryInfo queryInfo)
    {
        var query = _context.UserPointTransactions
            .AsNoTracking().OrderByDescending(t => t.CreatedAt)
            .Where(t => queryInfo.SearchText == null || EF.Functions.Collate(t.Description ?? "", "Latin1_General_CI_AI").Contains(queryInfo.SearchText))
            .Select(t => new UserPointTransaction
            {
                TransactionId = t.Id,
                PointsChanged = t.PointsChanged,
                NewTotal = t.NewTotal,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                User = new UserModels
                {
                    UserId = t.User!.Id,
                    UserName = t.User.Name
                },
                Package = new PackageModels
                {
                    PackageId = t.Package!.Id,
                    PackageName = t.Package.Name
                }
            });


        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var data = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<UserPointTransaction>
        {
            Data = data,
            TotalCount = total
        };
    }

    public async Task<QueryResult<UserPointTransaction>> GetTransactionsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        var query = _context.UserPointTransactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        (queryInfo.SearchText == null ||
                         EF.Functions.Collate(t.Description ?? "", "Latin1_General_CI_AI")
                             .Contains(queryInfo.SearchText)))
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new UserPointTransaction
            {
                TransactionId = t.Id,
                PointsChanged = t.PointsChanged,
                NewTotal = t.NewTotal,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                User = new UserModels
                {
                    UserId = t.User!.Id,
                    UserName = t.User.Name
                },
                Package = new PackageModels
                {
                    PackageId = t.Package!.Id,
                    PackageName = t.Package.Name
                }
            });

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
        var data = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<UserPointTransaction>
        {
            Data = data,
            TotalCount = total
        };
    }
}
