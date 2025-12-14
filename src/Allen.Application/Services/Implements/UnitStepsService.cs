//namespace Allen.Application;

//[RegisterService(typeof(IUnitStepsService))]
//public class UnitStepsService(
//    //IUnitStepsRepository _repository,
//    IUnitOfWork _unitOfWork,
//    IMapper _mapper) : IUnitStepsService
//{
//    public async Task<QueryResult<UnitStep>> GetUnitStepsOfUnitWithPagingAsync(Guid unitId, QueryInfo queryInfo)
//    {
//        return await _repository.GetUnitStepsOfUnitWithPagingAsync(unitId,queryInfo);
//    }
//    public async Task<UnitStep> GetByIdAsync(Guid id)
//    {
//        var entity = await _unitOfWork.Repository<UnitStepEntity>().GetByIdAsync(id);
//        return _mapper.Map<UnitStep>(entity);
//    }
//    public async Task<OperationResult> CreateAsync(CreateOrUpdateUnitStepModel model)
//    {
//        var maxIndex = await _repository.GetMaxStepIndexAsync(model.LearningUnitId);
//        model.StepIndex = maxIndex + 1;
//        var entity = _mapper.Map<UnitStepEntity>(model);
//        await _unitOfWork.Repository<UnitStepEntity>().AddAsync(entity);
//        if (!await _unitOfWork.SaveChangesAsync())
//        {
//            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(UnitStep)));
//        }
//        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(UnitStep)));
//    }
//    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateUnitStepModel model)
//    {
//        var entity = await _unitOfWork.Repository<UnitStepEntity>().GetByIdAsync(id);
//        if (entity == null)
//        {
//            return OperationResult.Failure($"UnitStep with Id {id} not found.");
//        }

//        entity.LearningUnitId = model.LearningUnitId;
//        entity.Title = model.Title;
//        entity.ContentJson = model.ContentJson;

//        _unitOfWork.Repository<UnitStepEntity>().UpdateAsync(entity);

//        if (!await _unitOfWork.SaveChangesAsync())
//        {
//            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(UnitStepEntity)));
//        }

//        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(UnitStepEntity)));
//    }

//    public async Task<OperationResult> DeleteAsync(Guid id)
//    {
//        await _unitOfWork.Repository<UnitStepEntity>().DeleteByIdAsync(id);
//        if (!await _unitOfWork.SaveChangesAsync())
//        {
//            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(UnitStep)));
//        }
//        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(UnitStep)));
//    }
//}
