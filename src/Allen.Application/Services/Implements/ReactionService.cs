namespace Allen.Application;

[RegisterService(typeof(IReactionService))]
public class ReactionService(
    INotificationService _notificationService,
    IReactionsRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper
) : IReactionService
{

    public async Task<IEnumerable<Reaction>> GetReactionsAsync(Guid objectId)
    {
        var objectType = await DetectObjectTypeAsync(objectId);
        if (objectType == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Reaction), objectId));

        return await _repository.GetReactionsAsync(objectId, objectType.Value);
    }

    public async Task<Reaction?> GetReactionByUserAsync(Guid userId, Guid objectId)
    {
        var objectType = await DetectObjectTypeAsync(objectId);
        if (objectType == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Reaction), objectId));

        return await _repository.GetReactionByUserAsync(userId, objectId, objectType.Value);
    }

    public async Task<IEnumerable<ReactionUserModel>> GetUsersByReactionAsync(Guid objectId, string reactionType)
    {
        var objectType = await DetectObjectTypeAsync(objectId);
        if (objectType == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Reaction), objectId));

        var parsedType = Enum.Parse<ReactionType>(reactionType, true);
        return await _repository.GetUsersByReactionAsync(objectId, objectType.Value, parsedType);
    }

    public async Task<OperationResult> CreateOrUpdateReactionAsync(CreateOrUpdateReactionModel model)
    {
        var objectType = await DetectObjectTypeAsync(model.ObjectId);
        if (objectType == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Reaction), model.ObjectId));

        var reaction = await _repository.GetReactionByUserAsync(model.UserId, model.ObjectId, objectType.Value);
        if (reaction != null)
        {
            var entity = await _unitOfWork.Repository<ReactionEntity>().GetByIdAsync(reaction.Id);
            if (entity.ReactionType.ToString() == model.ReactionType)
            {
                await _unitOfWork.Repository<ReactionEntity>().DeleteByIdAsync(reaction.Id);
                if (!await _unitOfWork.SaveChangesAsync())
                    return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(Reaction)));

                return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(Reaction)));
            }
            else
            {
                entity.ReactionType = Enum.Parse<ReactionType>(model.ReactionType!, true);

                _unitOfWork.Repository<ReactionEntity>().UpdateAsync(entity);
                if (!await _unitOfWork.SaveChangesAsync())
                    return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Reaction)));

                return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Reaction)));
            }
        }
        else
        {
            var entity = _mapper.Map<ReactionEntity>(model);
            entity.ObjectType = objectType.Value;
            entity.ReactionType = Enum.Parse<ReactionType>(model.ReactionType!, true);

            await _unitOfWork.Repository<ReactionEntity>().AddAsync(entity);
            if (!await _unitOfWork.SaveChangesAsync())
                return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Reaction)));

            // notification
            var post = await _unitOfWork.Repository<PostEntity>().GetByIdAsync(model.ObjectId);
            if (post != null)
            {
                if (post.UserId == model.UserId)
                    return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Reaction)));

                var notification = new NotificationModel
                {
                    UserId = model.UserId,
                    ReceiverId = post.UserId,
                    Title = "New reaction",
                    Message = $"{model.UserName} reacted with {model.ReactionType}",
                    EventType = NotificationEventType.PostReaction,
                    ObjectId = post.Id,
                    ObjectType = ObjectType.Post,
                    Payload = new { PostId = post.Id, ReactorId = model.UserId }
                };
                await _notificationService.NotifyAsync(notification);
            }

            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Reaction)));
        }
    }

    public async Task<IEnumerable<ReactionSummaryModel>> GetSummaryReactionAsync(Guid objectId)
    {
        var objectType = await DetectObjectTypeAsync(objectId);
        if (objectType == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Reaction), objectId));

        return await _repository.GetSummaryReactionAsync(objectId, objectType.Value);
    }

    private async Task<ObjectType?> DetectObjectTypeAsync(Guid objectId)
    {
        if (await _unitOfWork.Repository<PostEntity>().CheckExistByIdAsync(objectId))
            return ObjectType.Post;

        if (await _unitOfWork.Repository<UserEntity>().CheckExistByIdAsync(objectId))
            return ObjectType.User;

        if (await _unitOfWork.Repository<CommentEntity>().CheckExistByIdAsync(objectId))
            return ObjectType.Comment;

        return null;
    }
}