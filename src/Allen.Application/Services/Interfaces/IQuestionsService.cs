namespace Allen.Application;

public interface IQuestionsService
{
    Task<QueryResult<QuestionModel>> GetQuestionsAsync(Guid moduleItemId,  QueryInfo queryInfo);
    Task<QuestionModel> GetQuestionByIdAsync(Guid id);
    Task<List<AnswerResult>> GetAnswersOfUserByLearningIdAsync(Guid learningUnitId, Guid userId);
	Task<OperationResult> CreateQuestionAsync(CreateOrUpdateQuestionModel model);
    Task<OperationResult> CreateQuestionForReadingAsync(CreateOrUpdateQuestionForReadingModel model);
    Task<OperationResult> CreateQuestionForListeningAsync(CreateOrUpdateQuestionForListeningModel model);
    Task<OperationResult> CreateQuestionForSpeakingAsync(CreateOrUpdateQuestionForSpeakingModel model);
    Task<OperationResult> CreateQuestionsAsync(List<CreateOrUpdateQuestionModel> models);
    Task<OperationResult> UpdateQuestionAsync(Guid id, CreateOrUpdateQuestionModel model);
    Task<OperationResult> UpdateQuestionForReadingAsync(Guid id, CreateOrUpdateQuestionForReadingModel model);
    Task<OperationResult> UpdateQuestionForListeningAsync(Guid id, CreateOrUpdateQuestionForListeningModel model);
    Task<OperationResult> DeleteQuestionAsync(Guid id);
}
