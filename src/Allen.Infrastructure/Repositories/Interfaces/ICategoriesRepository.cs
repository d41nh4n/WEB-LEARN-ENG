namespace Allen.Infrastructure;

public interface ICategoriesRepository : IRepositoryBase<CategoryEntity>
{
    Task<QueryResult<CategoryModel>> GetCategoriesAsync(CategoryQuery categoryQuery);
    Task<CategoryModel> GetCategoryByIdAsync(Guid id);
}
