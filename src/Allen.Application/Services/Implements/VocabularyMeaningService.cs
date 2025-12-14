namespace Allen.Application;

[RegisterService(typeof(IVocabularyMeaningService))]
public class VocabularyMeaningService(
    IVocabularyMeaningRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IVocabularyMeaningService
{
    // Create batch meanings (for a vocabulary)
    public async Task<OperationResult> CreateVocabularyMeaningsAsync(Guid vocabularyId, List<CreateVocabularyMeaningModel>? meaningModels)
    {

        var meaningEntities = _mapper.Map<List<VocabularyMeaningEntity>>(meaningModels)
                                     .Select(m => { m.VocabularyId = vocabularyId; return m; })
                                     .ToList();

        await _unitOfWork.Repository<VocabularyMeaningEntity>().AddRangeAsync(meaningEntities);

        return OperationResult.SuccessResult(ErrorMessageBase.CreatedSuccess, meaningEntities.Count);
    }

    // Delete all meanings for a vocabulary
    public async Task<OperationResult> DeleteVocabMeaningByVocabIdAsync(Guid vocabId)
    {
        var existing = (await _repository.GetVocabMeaningByVocabIdAsync(vocabId)).ToList();

        if (existing.Count <= 0)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyMeaningEntity), vocabId));

        _unitOfWork.Repository<VocabularyMeaningEntity>().DeleteRangeAsync(existing);
        return OperationResult.SuccessResult(ErrorMessageBase.DeletedSuccess, existing.Count);
    }

    // Update meanings: simple strategy = delete all old, add new (atomic)
    public async Task<OperationResult> UpdateVocabularyMeaningsAsync(Guid vocabularyId, List<UpdateVocabularyMeaningModel>? meaningModels)
    {
        var existing = (await _repository.GetVocabMeaningByVocabIdAsync(vocabularyId)).ToList();
        if (existing.Count != 0)
            _unitOfWork.Repository<VocabularyMeaningEntity>().DeleteRangeAsync(existing);

        var toInsert = _mapper.Map<List<VocabularyMeaningEntity>>(meaningModels) ?? [];
        foreach (var m in toInsert)
            m.VocabularyId = vocabularyId;

        if (toInsert.Count != 0)
            await _unitOfWork.Repository<VocabularyMeaningEntity>().AddRangeAsync(toInsert);

        return OperationResult.SuccessResult(ErrorMessageBase.UpdatedSuccess);
    }

    // Delete single meaning by id
    public async Task<OperationResult> DeleteVocabularyMeaningAsync(Guid meaningId)
    {
        var entity = await _unitOfWork.Repository<VocabularyMeaningEntity>().GetByIdAsync(meaningId);
        if (entity == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyMeaningEntity), meaningId));

        try
        {
            await _unitOfWork.Repository<VocabularyMeaningEntity>().DeleteByIdAsync(entity.Id);
            return OperationResult.SuccessResult(ErrorMessageBase.DeletedSuccess, meaningId);
        }
        catch (Exception ex)
        {
            throw new InternalServerException(ex.Message, nameof(VocabularyMeaningService));
        }
    }

    // Get meaning by id (model)
    public async Task<VocabularyMeaningModel?> GetVocabularyMeaningByIdAsync(Guid id)
    {
        var e = await _unitOfWork.Repository<VocabularyMeaningEntity>().GetByIdAsync(id);
        return _mapper.Map<VocabularyMeaningModel>(e) 
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyMeaningEntity), id)); ;
    }

    // Get meanings of a vocabulary
    public async Task<IEnumerable<VocabularyMeaningModel>> GetVocabularyMeaningsByVocabularyIdAsync(Guid vocabularyId)
    {
        var entities = await _repository.GetVocabMeaningByVocabIdAsync(vocabularyId);
        return _mapper.Map<List<VocabularyMeaningModel>>(entities);
    }
}
