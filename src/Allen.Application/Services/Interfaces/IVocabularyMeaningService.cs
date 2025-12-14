namespace Allen.Application;

public interface IVocabularyMeaningService
{
    Task<OperationResult> DeleteVocabMeaningByVocabIdAsync(Guid vocabId);
    Task<OperationResult> CreateVocabularyMeaningsAsync(Guid vocabularyId, List<CreateVocabularyMeaningModel>? meaningModels);
    Task<OperationResult> UpdateVocabularyMeaningsAsync(Guid vocabularyId, List<UpdateVocabularyMeaningModel>? meaningModels);
    Task<OperationResult> DeleteVocabularyMeaningAsync(Guid meaningId);
    Task<VocabularyMeaningModel?> GetVocabularyMeaningByIdAsync(Guid id);
    Task<IEnumerable<VocabularyMeaningModel>> GetVocabularyMeaningsByVocabularyIdAsync(Guid vocabularyId);
}
