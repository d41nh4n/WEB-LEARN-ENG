using StackExchange.Redis;

namespace Allen.Infrastructure
{
    public interface IWritingRepository : IRepositoryBase<WritingEntity>
    {
        // -- Learning --
        Task<QueryResult<WritingLearningModel>> GetLearningWritingsAsync(QueryInfo queryInfo);
        Task<WritingLearningModel> GetLearningWritingByLearningUnitIdAsync(Guid id);
        Task<WritingLearningModel> GetLearningWritingByIdAsync(Guid id);

        // -- Ielts --
        Task<QueryResult<WritingIeltsModel>> GetIeltsWritingsAsync(QueryInfo queryInfo);
        Task<WritingIeltsModel> GetIeltsWritingByLearningUnitIdAsync(Guid id);
        Task<WritingIeltsModel> GetIeltsWritingByIdAsync(Guid id);
    }
}