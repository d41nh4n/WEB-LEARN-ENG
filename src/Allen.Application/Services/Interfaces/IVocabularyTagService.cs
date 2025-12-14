namespace Allen.Application;

public interface IVocabularyTagService
{
    Task<IEnumerable<VocabularyTagModel>> GetVocabularyTagsModelByVocabularyIdAsync(Guid vocabularyId);
    Task<IEnumerable<VocabularyTagEntity>> GetVocabularyTagsEntityByVocabularyIdAsync(Guid vocabularyId);
    Task<OperationResult> AddVocabularyTagToVocabularyAsync(Guid vocabularyId, Guid tagId);
    Task<OperationResult> CreateVocabularyTagsAsync(Guid vocabularyId, List<Guid> tagsId);
    Task<OperationResult> UpdateVocabularyTagsAsync(Guid vocabularyId, List<Guid> tagsId);
    Task<OperationResult> RemoveVocabularyTagsFromVocabularyAsync(List<Guid> tagsId, Guid vocabularyId);
    Task<OperationResult> RemoveAllVocabularyTagsFromVocabularyAsync(Guid vocabularyId);
    Task<OperationResult> RemoveVocabularyTagFromAllVocabulariesAsync(Guid tagId);
    Task<OperationResult> AddTagsExistedIntoVocabularyAsync(List<Guid> tagsId, Guid vocabularyId);
    Task<List<Guid>> GetTagsIdByVocabularyIdAsync(Guid vocabularyId);
}
