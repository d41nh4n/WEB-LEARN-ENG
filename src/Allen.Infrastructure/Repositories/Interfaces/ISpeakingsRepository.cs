namespace Allen.Infrastructure;

public interface ISpeakingsRepository : IRepositoryBase<SpeakingEntity>
{
    Task<SpeakingModel> GetByLearningUnitIdAsync(Guid learningUnitId);
	Task<List<SpeakingForIeltsModel>> GetByLearningUnitIdForIeltsAsync(Guid learningUnitId, GetByLearningUnitIdForIeltsQuery query);

	Task<TranscriptForSubmitModel> GetTranscriptByIdAsync(Guid transcriptId);
}
