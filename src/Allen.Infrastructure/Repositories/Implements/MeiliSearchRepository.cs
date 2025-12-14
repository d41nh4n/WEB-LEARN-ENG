using Meilisearch;
using static Meilisearch.TypoTolerance;

namespace Allen.Infrastructure;

[RegisterService(typeof(IMeiliSearchRepository<>))]
public class MeiliSearchRepository<T> : IMeiliSearchRepository<T>
{
    private readonly MeilisearchClient _client;
    private readonly string _indexName;

    public MeiliSearchRepository(MeilisearchClient client)
    {
        _client = client;
        _indexName = typeof(T).Name.ToLower();
    }

    private async Task<Meilisearch.Index> GetOrCreateIndexAsync()
    {
        try
        {
            return await _client.GetIndexAsync(_indexName);
        }
        catch (MeilisearchApiError)
        {
            Meilisearch.Index newIndex = _client.Index(_indexName);

            // 🔧 Cấu hình fuzzy search
            var settings = new Settings
            {
                TypoTolerance = new TypoTolerance
                {
                    Enabled = true,
                    MinWordSizeForTypos = new TypoSize
                    {
                        OneTypo = 6,  // >=6 ký tự mới cho phép 1 lỗi
                        TwoTypos = 10 // >=10 ký tự mới cho phép 2 lỗi
                    },
                    DisableOnAttributes = new[] { "id" }
                },
                SearchableAttributes = new[] { "word" },
                DisplayedAttributes = new[] { "id", "word" },
                RankingRules = new[]
                {
                    "exactness",
                    "words",
                    "typo",
                    "proximity",
                    "attribute",
                    "sort"
                }
            };

            await newIndex.UpdateSettingsAsync(settings);
            return newIndex;
        }
    }

    public async Task IndexAsync(T entity)
    {
        var index = await GetOrCreateIndexAsync();
        await index.AddDocumentsAsync(new[] { entity });
    }

    public async Task DeleteAsync(Guid id)
    {
        var index = await GetOrCreateIndexAsync();
        await index.DeleteOneDocumentAsync(id.ToString());
    }

    public async Task<IEnumerable<VocabularyMLSModel>> SearchAsync(string word)
    {
        var index = await GetOrCreateIndexAsync();
        var result = await index.SearchAsync<VocabularyMLSModel>(
                    word,
                    new SearchQuery()
                    {
                        Limit = 5,
                        AttributesToRetrieve = new[] { "id", "word" },
                        MatchingStrategy = "all"
                    }
                );
        return result.Hits;
    }

    public async Task<TaskResource> AddVocabularyAsync(VocabularyMLSModel vocabularyMLSModel)
    {
        var index = await GetOrCreateIndexAsync();

        var task = await index.AddDocumentsAsync(new[] { vocabularyMLSModel });

        await _client.WaitForTaskAsync(task.TaskUid);

        var taskInfo = await _client.GetTaskAsync(task.TaskUid);

        if (taskInfo.Status == TaskInfoStatus.Failed)
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, vocabularyMLSModel!.Word!);
        }

        return taskInfo;
    }

    public Meilisearch.MeilisearchClient GetCurrentClientAsync()
    {
        return this._client;
    }

    public async Task<TaskResource> UpdateVocabularyAsync(VocabularyMLSModel vocabularyMLSModel)
    {
        var index = await GetOrCreateIndexAsync();

        var task = await index.UpdateDocumentsAsync(new[] { vocabularyMLSModel });

        await _client.WaitForTaskAsync(task.TaskUid);

        var taskInfo = await _client.GetTaskAsync(task.TaskUid);

        if (taskInfo.Status == TaskInfoStatus.Failed)
        {
            throw new InternalServerException(ErrorMessageBase.CreateFailure, vocabularyMLSModel!.Word!);
        }

        return taskInfo;
    }

}
