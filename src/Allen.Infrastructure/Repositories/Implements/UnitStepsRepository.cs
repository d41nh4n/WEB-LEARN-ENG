//namespace Allen.Infrastructure;

//[RegisterService(typeof(IUnitStepsRepository))]
//public class UnitStepsRepository(SqlApplicationDbContext context) : RepositoryBase<UnitStepEntity>(context), IUnitStepsRepository
//{
//    private readonly SqlApplicationDbContext _context = context;

//    public async Task<QueryResult<UnitStep>> GetUnitStepsOfUnitWithPagingAsync(Guid unitId, QueryInfo queryInfo)
//    {
//        var query = from unitStep in _context.UnitSteps.AsNoTracking()
//                    where unitStep.LearningUnitId == unitId
//                    join userStepProgresses in _context.UserStepProgresses
//                        on unitStep.Id equals userStepProgresses.UnitStepId into userStepProgressesGroup
//                    select new UnitStep
//                    {
//                        Id = unitStep.Id,
//                        StepIndex = unitStep.StepIndex,
//                        Title = unitStep.Title,
//                    };

//        var entities = await query
//            .Skip(queryInfo.Skip)
//            .Take(queryInfo.Top)
//            .ToListAsync();

//        var totalCount = 0;
//        if (queryInfo.NeedTotalCount)
//        {
//            totalCount = await query.CountAsync();
//        }
//        return new QueryResult<UnitStep>
//        {
//            Data = entities,
//            TotalCount = totalCount
//        };
//    }

//    public async Task<int> GetMaxStepIndexAsync(Guid learningUnitId)
//    {
//        return await _context.UnitSteps
//            .Where(x => x.LearningUnitId == learningUnitId)
//            .Select(x => (int?)x.StepIndex)
//            .MaxAsync() ?? 0;
//    }
//}
