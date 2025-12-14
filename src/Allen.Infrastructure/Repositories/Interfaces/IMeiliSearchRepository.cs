using Meilisearch;

namespace Allen.Infrastructure;

public interface IMeiliSearchRepository<T>
{
    Task IndexAsync(T entity);
    Task<IEnumerable<VocabularyMLSModel>> SearchAsync(string query);
    Task DeleteAsync(Guid id);
    Task<TaskResource> AddVocabularyAsync(VocabularyMLSModel vocabularyMLSModel);
    Meilisearch.MeilisearchClient GetCurrentClientAsync();
    Task<TaskResource> UpdateVocabularyAsync(VocabularyMLSModel vocabularyMLSModel);
}