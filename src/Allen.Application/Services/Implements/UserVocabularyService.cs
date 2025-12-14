
namespace Allen.Application;
[RegisterService(typeof(IUserVocabularyService))]

public class UserVocabularyService
     (IUserVocabularyRepository _repository,
    IUnitOfWork _unitOfWork) : IUserVocabularyService
{
    public async Task<OperationResult> AddVocabularyToUserAsync(Guid userId, Guid vocabularyId)
    {
        if (_unitOfWork.Repository<UserVocabularyEntity>().CheckExistAsync(x => x.UserId == userId && x.VocabularyId == vocabularyId).Result)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(UserVocabularyEntity), vocabularyId));

        var entity = new UserVocabularyEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VocabularyId = vocabularyId
        };

        await _unitOfWork.Repository<UserVocabularyEntity>().AddAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            throw new InternalServerException(ErrorMessageBase.CreateFailure, nameof(UserVocabularyEntity));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(UserVocabularyEntity)), "Add Vocabulary Success");
    }

    public async Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserAsync(QueryInfo queryInfo, Guid userId)
    {
        var result =  await _repository.GetVocabulariesOfUserAsync(queryInfo, userId);
        return result == null
            ? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyOfUserModel), userId))
            : result;
    }

    public async Task<QueryResult<VocabularyOfUserModel>> GetVocabulariesOfUserByTopicIdAsync(QueryInfo queryInfo, Guid userId, Guid topicId)
    {
        var result = await _repository.GetVocabulariesOfUserByTopicIdAsync(queryInfo, userId, topicId);
        return result == null
            ? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyOfUserModel), userId))
            : result;
    }

    public async Task<OperationResult> RemoveVocabularyFromUserAsync(Guid userId, Guid vocabularyId)
    {
        var entity = await _unitOfWork.Repository<UserVocabularyEntity>()
            .GetByConditionAsync(x => x.UserId == userId && x.VocabularyId == vocabularyId);

        if(entity == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(UserVocabularyEntity), vocabularyId));

        await _unitOfWork.Repository<UserVocabularyEntity>().DeleteByIdAsync(entity.Id!);

        if (!await _unitOfWork.SaveChangesAsync())
            throw new InternalServerException(ErrorMessageBase.DeleteFailure, nameof(UserVocabularyEntity));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(UserVocabularyEntity)), "Remove Vocabulary Success");
    }
}

