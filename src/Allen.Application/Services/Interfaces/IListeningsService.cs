namespace Allen.Application;

public interface IListeningsService
{
	Task<ListeningModel> GetByLearningUnitIdAsync(Guid learningUnitId);
	Task<ListeningForIeltsModel> GetByLearningUnitIdForIeltsAsync(Guid learningUnitId, GetByLearningUnitIdForIeltsQuery query);
	Task<OperationResult> CreateLearningAsync(CreateListeningForLearningModel model);
	Task<OperationResult> CreateListeningForIeltsAsync(CreateListeningForIeltsModel model);
	Task<OperationResult> CreateListeningsForIeltsAsync(CreateListeningsForIeltsModel model);
	Task<OperationResult> SubmitIeltsAsync(SubmitIeltsModel model);

	Task<OperationResult> UpdateLearningAsync(Guid id, UpdateListeningForLearningModel model);
	Task<OperationResult> UpdateIeltsAsync(Guid id, UpdateListeningForIeltsModel model);
	Task<OperationResult> DeleteAsync(Guid id);
}
