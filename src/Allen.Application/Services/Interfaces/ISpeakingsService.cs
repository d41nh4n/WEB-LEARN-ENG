namespace Allen.Application;

public interface ISpeakingsService
{
	Task<SpeakingModel> GetByLearningUnitIdAsync(Guid learningUnitId);
	Task<List<SpeakingForIeltsModel>> GetByLearningUnitIdForIeltsAsync(Guid learningUnitId, GetByLearningUnitIdForIeltsQuery query);
	Task<OperationResult> CreateLearningAsync(CreateSpeakingModel model);
	Task<OperationResult> CreateIeltsAsync(CreateOrUpdateSpeakingIeltsModel model);
	Task<OperationResult> UpdateLearningAsync(Guid id, UpdateSpeakingModel model);
	Task<OperationResult> UpdateIeltsAsync(Guid id, CreateOrUpdateSpeakingIeltsModel model);
	Task<OperationResult> SubmitAsync(SubmitSpeakingModel model);
	Task<OperationResult> SubmitIeltsAsync(Guid userId, SubmitSpeakingIeltsModel model);
}