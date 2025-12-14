namespace Allen.Infrastructure;

public interface IQuestionsRepository : IRepositoryBase<QuestionEntity>
{
    Task<QueryResult<QuestionModel>> GetQuestionsAsync(Guid moduleItemId, QueryInfo queryInfo);
    Task<QuestionModel> GetQuestionByIdAsync(Guid id);
    Task<List<QuestionEntity>> GetQuestionsByLearningUnitIdAsync(Guid learningUnitId, bool readingOrListening);
    Task<List<SubQuestionEntity>> GetSubQuestionsByLearningUnitIdAsync(Guid learningUnitId, bool readingOrListening);
    Task<List<AnswerResult>> GetAnswersOfUserByLearningIdAsync(Guid learningUnitId, Guid userId);
}
