using Meilisearch;

namespace Allen.Application;

public interface IMeiliSearchService<T>
{
    Task IndexAsync(T entity);
    Task<List<VocabularyMLSModel>> SearchAsync(string query);
    Task DeleteAsync(Guid id);
    Task<TaskResource> AddVocabularyAsync(VocabularyMLSModel vocabularyMLSModel);
    Task<TaskResource> UpdateVocabularyAsync(VocabularyMLSModel vocabularyMLSModel);
}