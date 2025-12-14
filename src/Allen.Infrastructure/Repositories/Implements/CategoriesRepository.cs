namespace Allen.Infrastructure;

[RegisterService(typeof(ICategoriesRepository))]
public class CategoriesRepository(SqlApplicationDbContext context) : RepositoryBase<CategoryEntity>(context), ICategoriesRepository
{
    public readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<CategoryModel>> GetCategoriesAsync(CategoryQuery categoryQuery)
    {
        if (!Enum.TryParse(categoryQuery.SkillType, true, out SkillType skillEnum))
        {
            throw new ArgumentException("Invalid skill type");
        }
        var query = _context.Categories.AsNoTracking()
            .Where(x => x.Name.Contains(categoryQuery.QueryInfo.SearchText ?? ""))
            .Where(x => x.SkillType == skillEnum);

        var entities = await query
            .OrderByDescending(x => categoryQuery.QueryInfo.OrderBy)
            .Skip(categoryQuery.QueryInfo.Skip)
            .Take(categoryQuery.QueryInfo.Top)
            .Select(x => new CategoryModel
            {
                Id = x.Id,
                Name = x.Name,
                SkillType = x.SkillType.ToString(),
                CreateAt = x.CreatedAt
			})
            .ToListAsync();
        var total = categoryQuery.QueryInfo.NeedTotalCount ? await query.CountAsync() : 0;
        return new QueryResult<CategoryModel>
        {
            Data = entities,
            TotalCount = total
        };
    }

    public async Task<CategoryModel> GetCategoryByIdAsync(Guid id)
    {
        return await _context.Categories.AsNoTracking()
             .Where(x => x.Id == id)
             .Select(x => new CategoryModel
             {
                 Id = x.Id,
                 Name = x.Name,
                 SkillType = x.SkillType.ToString()
             })
             .FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(CategoryEntity), id));
    }
}
