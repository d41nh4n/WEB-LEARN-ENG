namespace Allen.Application;

[RegisterService(typeof(IUserPointService))]
public class UserPointService(IUserPointRepository _repository, IUnitOfWork _unitOfWork, INotificationService _notificationService) : IUserPointService
{
    public async Task<QueryResult<UserPoint>> GetAllUserPointsAsync(QueryInfo queryInfo)
    {
        return await _repository.GetAllUserPointsAsync(queryInfo);
    }

    public async Task<OperationResult> GetUserPointsByUserIdAsync(Guid userId)
    {
        await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        var userPoint = await _repository.GetUserPointByUserIdAsync(userId);
        if (userPoint == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(UserPointEntity), userId));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.RetrievedSuccess, nameof(UserPointEntity)), userPoint);
    }

    public async Task<OperationResult> AddPointsAsync(Guid userId, PackageModel package, Guid paymentId)
    {
        await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        var userPoint = await _unitOfWork.Repository<UserPointEntity>().GetByConditionAsync(x => x.UserId == userId);

        // chua co thi tao moi, co roi thi cap nhat diem
        if (userPoint == null)
        {
            userPoint = new UserPointEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalPoints = package.Points
            };
            await _unitOfWork.Repository<UserPointEntity>().AddAsync(userPoint);
        }
        else
        {
            userPoint.TotalPoints += package.Points;
            _unitOfWork.Repository<UserPointEntity>().UpdateAsync(userPoint);
        }

        var transaction = new UserPointTransactionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PackageId = package.Id,
            PaymentId = paymentId,
            PointsChanged = package.Points,
            NewTotal = userPoint.TotalPoints,
            Description = $"Buy package {package.Name}",
            ActionType = "BUY_PACKAGE"
        };

        await _unitOfWork.Repository<UserPointTransactionEntity>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        var notification = new NotificationModel
        {
            ReceiverId = userId,
            Title = "Points Added",
            Message = $"You have just been added {package.Points} points.",
            EventType = NotificationEventType.PointsAdded,
            ObjectId = transaction.Id,
            ObjectType = ObjectType.UserPointTransaction,
            Payload = new { transaction.Id, transaction.NewTotal }
        };
        await _notificationService.NotifyAsync(notification);

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(UserPointEntity)));
    }

    public async Task<OperationResult> UsePointsAsync(Guid userId, int pointsToUse)
    {
        await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        var userPoint = await _unitOfWork.Repository<UserPointEntity>().GetByConditionAsync(x => x.UserId == userId);
        if (userPoint == null)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(UserPointEntity), userId));

        if (userPoint.TotalPoints < pointsToUse)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.Invalid, "Insufficient points balance"));

        userPoint.TotalPoints -= pointsToUse;
        _unitOfWork.Repository<UserPointEntity>().UpdateAsync(userPoint);
        await _unitOfWork.SaveChangesAsync(); ;

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(UserPointEntity)));
    }

    public async Task<OperationResult> AddPointsInternalAsync(AddPointsModel model)
    {
        await _unitOfWork.Repository<UserEntity>().GetByIdAsync(model.UserId);
        var userPoint = await _unitOfWork.Repository<UserPointEntity>().GetByConditionAsync(x => x.UserId == model.UserId);

        if (userPoint == null)
        {
            userPoint = new UserPointEntity
            {
                Id = Guid.NewGuid(),
                UserId = model.UserId,
                TotalPoints = model.Points
            };
            await _unitOfWork.Repository<UserPointEntity>().AddAsync(userPoint);
        }
        else
        {
            userPoint.TotalPoints += model.Points;
            _unitOfWork.Repository<UserPointEntity>().UpdateAsync(userPoint);
        }


        var bonusPackage = await _unitOfWork.Repository<PackageEntity>().GetByConditionAsync(x => x.Name == "ADMIN_BONUS");
        if (bonusPackage == null)
        {
            bonusPackage = new PackageEntity
            {
                Id = Guid.NewGuid(),
                Name = "ADMIN_BONUS",
                Description = "Package for admin bonus points",
                Points = 0,
                Price = 0,
                IsActive = true
            };
            await _unitOfWork.Repository<PackageEntity>().AddAsync(bonusPackage);
            await _unitOfWork.SaveChangesAsync();
        }

        var transaction = new UserPointTransactionEntity
        {
            Id = Guid.NewGuid(),
            UserId = model.UserId,
            PackageId = bonusPackage.Id,
            PointsChanged = model.Points,
            NewTotal = userPoint.TotalPoints,
            Description = model.Description,
            ActionType = "BONUS"
        };

        await _unitOfWork.Repository<UserPointTransactionEntity>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        var notification = new NotificationModel
        {
            ReceiverId = model.UserId,
            Title = "Points Added",
            Message = $"You have just been given {model.Points} bonus points.",
            EventType = NotificationEventType.PointsAdded,
            ObjectId = transaction.Id,
            ObjectType = ObjectType.UserPointTransaction,
            Payload = new { transaction.Id, transaction.NewTotal }
        };
        await _notificationService.NotifyAsync(notification);

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(UserPointEntity)));
    }

    public async Task<bool> UsePointsInternalAsync(Guid userId, int pointsToUse)
    {
        await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        var userPoint = await _unitOfWork.Repository<UserPointEntity>().GetByConditionAsync(x => x.UserId == userId);

        if (userPoint == null)
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(UserPointEntity), userId));

        if (userPoint.TotalPoints < pointsToUse)
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.InsufficientPoints, userPoint.TotalPoints, pointsToUse));
        
        userPoint.TotalPoints -= pointsToUse;

        _unitOfWork.Repository<UserPointEntity>().UpdateAsync(userPoint);
        return true;
    }
}
