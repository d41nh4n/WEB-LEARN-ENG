namespace Allen.Application;

[RegisterService(typeof(ICommentsService))]
public class CommentsService(
    INotificationService _notificationService,
    ICommentsRepository _repository,
    IRolesService _rolesService,
    IUnitOfWork _unitOfWork,
    IMapper _mapper
) : ICommentsService
{
    public async Task<QueryResult<Comment>> GetRootCommentsAsync(Guid objectId, QueryInfo queryInfo)
    {
        var objectExists = await DetectObjectTypeAsync(objectId);
        if (objectExists == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Comment), objectId));
        return await _repository.GetRootCommentsAsync(objectId, queryInfo);
    }

    public async Task<QueryResult<Comment>> GetRepliesAsync(Guid parentId, QueryInfo queryInfo)
    {
        var rootComment = await _unitOfWork.Repository<CommentEntity>().GetByIdAsync(parentId);
        return await _repository.GetRepliesAsync(parentId, queryInfo);
    }

    public async Task<Comment> GetCommentByIdAsync(Guid id)
    {
        var entity = await _repository.GetCommentByIdAsync(id);
        return _mapper.Map<Comment>(entity);
    }

    public async Task<OperationResult> CreateAsync(CreateCommentModel model)
    {
        var entity = _mapper.Map<CommentEntity>(model);
        entity.Id = Guid.NewGuid();

        var objectType = await DetectObjectTypeAsync(model.ObjectId);
        if (objectType == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Comment), model.ObjectId));
        entity.ObjectType = objectType.Value;

        if (model.CommentParentId == null)
        {
            entity.CommentParentId = entity.Id;
            entity.Parent = null;
        }
        else
        {
            // Reply comment → tìm parent
            var parent = await _unitOfWork.Repository<CommentEntity>().GetByIdAsync(model.CommentParentId.Value);
            entity.Parent = parent;
            entity.CommentParentId = parent.CommentParentId;
        }

        await _unitOfWork.Repository<CommentEntity>().AddAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Comment)));

        // notification
        var post = await _unitOfWork.Repository<PostEntity>().GetByIdAsync(model.ObjectId);
        if (post != null)
        {
            if (post.UserId == model.UserId)
                return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Comment)), new { entity.Id });

            var notification = new NotificationModel
            {
                UserId = model.UserId,
                ReceiverId = post.UserId,
                Title = "New comment",
                Message = $"{model.UserName} added a new comment",
                EventType = NotificationEventType.PostComment,
                ObjectId = post.Id,
                ObjectType = ObjectType.Post,
                Payload = new { PostId = post.Id, CommentId = entity.Id, CommenterId = model.UserId }
            };
            await _notificationService.NotifyAsync(notification);
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Comment)), new { entity.Id });
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpdateCommentModel model)
    {
        var entity = await _unitOfWork.Repository<CommentEntity>().GetByIdAsync(id);

        if (entity.UserId != model.UserId)
            return OperationResult.Failure(ErrorMessageBase.Forbidden);

        entity.Content = model.Content;
        _unitOfWork.Repository<CommentEntity>().UpdateAsync(entity);

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Comment)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Comment)));
    }

    public async Task<OperationResult> DeleteAsync(Guid id, Guid userId)
    {
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var entity = await _repository.GetCommentByIdAsync(id);

                var roles = await _rolesService.GetRoleForUserAsync(userId);
                if (entity.UserId != userId && !roles.Any(r => r.Name == "Admin"))
                    throw new ForbiddenException(ErrorMessageBase.Forbidden);

                var commentsToDelete = new List<CommentEntity>();

                if (entity.Id == entity.CommentParentId)
                {
                    // comment root → get all replies
                    commentsToDelete = await _repository.GetCommentsByParentIdAsync(entity.CommentParentId);
                }
                else
                {
                    // comment con → chuyển replies lên parent
                    if (entity.Parent != null)
                    {
                        await _repository.ReassignRepliesParentAsync(entity, entity.Parent);
                    }
                    commentsToDelete.Add(entity);
                }

                if (commentsToDelete.Any())
                {
                    var commentIds = commentsToDelete.Select(c => c.Id).ToList();
                    var commentReactions = await _unitOfWork.Repository<ReactionEntity>()
                        .GetListByConditionAsync(r => commentIds.Contains(r.ObjectId) && r.ObjectType == ObjectType.Comment);
                    _unitOfWork.Repository<ReactionEntity>().DeleteRangeAsync(commentReactions);
                    _unitOfWork.Repository<CommentEntity>().DeleteRangeAsync(commentsToDelete);
                }
            });

            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(Comment)));
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(Comment)) + ": " + ex.Message);
        }
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