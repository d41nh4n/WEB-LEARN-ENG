namespace Allen.Application;

public interface ICategoriesService
{
    Task<QueryResult<CategoryModel>> GetCategoriesAsync(CategoryQuery categoryQuery);
    Task<CategoryModel> GetByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateOrUpdateCategoryModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateCategoryModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
