namespace Allen.Infrastructure;

public interface IListeningsRepository : IRepositoryBase<ListeningEntity>
{
    Task<ListeningEntity> GetById(Guid id);
    Task<ListeningModel> GetByLearningUnitId(Guid learningUnitId);
    Task<ListeningForIeltsModel> GetByLearningUnitIdForIeltsAsync(Guid learningUnitId, GetByLearningUnitIdForIeltsQuery query);
    Task<LearningUnitEntity> GetListeningUnitWithQuestionsAsync(Guid id);
}
