using Allen.Domain;

namespace Allen.Infrastructure;

[RegisterService(typeof(IPackageRepository))]
public class PackageRepository(
    SqlApplicationDbContext context
) : RepositoryBase<PackageEntity>(context), IPackageRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<PackageModel>> GetPackagesAsync(QueryInfo queryInfo, PackageQuery packageQuery)
    {
        var query = _context.Packages.AsNoTracking();

        // Filter by IsActive (true/false)
        if (packageQuery?.IsActive.HasValue == true)
        {
            query = query.Where(p => p.IsActive == packageQuery.IsActive.Value);
        }

        // Search by Name or Description)
        if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
        {
            var search = queryInfo.SearchText.Trim();

            query = query.Where(p =>
                EF.Functions.Collate(p.Name ?? "", "Latin1_General_CI_AI").Contains(search) ||
                EF.Functions.Collate(p.Description ?? "", "Latin1_General_CI_AI").Contains(search));
        }

        // Total count
        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        // Sorting
        var orderedQuery = string.IsNullOrEmpty(queryInfo.OrderBy)
            ? query.OrderByDescending(p => p.CreatedAt) // default sort
            : queryInfo.OrderType == OrderType.Ascending
                ? query.OrderBy(e => EF.Property<object>(e, queryInfo.OrderBy))
                : query.OrderByDescending(e => EF.Property<object>(e, queryInfo.OrderBy));

        // Paging
        var packages = await orderedQuery
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(p => new PackageModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Points = p.Points,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return new QueryResult<PackageModel>
        {
            Data = packages,
            TotalCount = total
        };
    }

    public async Task<PackageEntity?> FindAsync(Expression<Func<PackageEntity, bool>> predicate)
    {
        return await _context.Set<PackageEntity>().FirstOrDefaultAsync(predicate);
    }
}
