namespace Allen.Application;

[RegisterService(typeof(IUsersService))]
public class UsersService(
	IUsersRepository _repository,
	IUserPointService _userPointService,
	IUnitOfWork _unitOfWork,
	IAppConfiguration _configuration,
	IBlobStorageService _blob,
	ILogger<UsersService> _logger,
	IMapper _mapper) : IUsersService
{
	private async Task<List<UnitItem>> RecommendUnitsForSkillAsync(SkillType skill, double band, Guid userId)
	{
		var level = MapBandToLevel(band);

		var units = await _repository.GetUnitsForSkillAsync(skill, level, userId);

		return units.Select(u => new UnitItem
		{
			Id = u.Id,
			Title = u.Title,
			Description = u.Description,
			Level = u.Level.ToString(),
			Status = u.LearningUnitStatusType.ToString()
		}).ToList();
	}

	public async Task<List<RecommendedUnitsResponse>> RecommendAsync(
		UserBandModel band,
		Guid userId)
	{
		var result = new List<RecommendedUnitsResponse>();

		async Task Add(SkillType skill, double score)
		{
			result.Add(new RecommendedUnitsResponse
			{
				Skill = skill.ToString(),
				Units = await RecommendUnitsForSkillAsync(skill, score, userId)
			});
		}

		await Add(SkillType.Reading, band.Reading);
		await Add(SkillType.Listening, band.Listening);
		await Add(SkillType.Speaking, band.Speaking);
		await Add(SkillType.Writing, band.Writing);

		return result;
	}
	public async Task<OperationResult> CreateAsync(RegisterModel model)
	{
		if (await _unitOfWork.Repository<UserEntity>().CheckExistAsync(x => x.Email == model.Email))
		{
			throw new BadRequestException(string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Email));
		}

		if (await _unitOfWork.Repository<UserEntity>().CheckExistAsync(x => x.Name == model.Name))
		{
			throw new BadRequestException(string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Name));
		}

		model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

		var userEntity = _mapper.Map<UserEntity>(model);

		userEntity.IsDeleted = false;
		var result = await _unitOfWork.Repository<UserEntity>().AddAsync(userEntity);
		if (result == null)
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(User)));

		var role = await _unitOfWork.Repository<RoleEntity>().GetByConditionAsync(x => x.Name == RoleType.User);
		if (role == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role), RoleType.User));

		var userRole = new UserRoleEntity
		{
			Id = Guid.NewGuid(),
			UserId = result.Id,
			RoleId = role.Id
		};
		try
		{
			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				await _unitOfWork.Repository<UserRoleEntity>().AddAsync(userRole);
				await _userPointService.AddPointsInternalAsync(new AddPointsModel
				{
					UserId = result.Id,
					Points = 10,
					Description = "Welcome bonus points for registering an account"
				});

			});
			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(User)));
		}
		catch (Exception ex)
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(User)));
		}
	}
	public async Task<OperationResult> UpdateAsync(Guid id, UpdateUserModel model)
	{
		string? newBlobName = null;
		string? oldBlobName = null;

		try
		{
			_logger.LogInformation(LogMessageBase.Processing, nameof(User), id);

			var user = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(id);
			if (user == null)
			{
				return OperationResult.Failure("User not found.");
			}

			if (model.Picture != null)
			{
				if (!string.IsNullOrEmpty(user.Picture))
				{
					oldBlobName = user.Picture;
				}

				var blobResult = await _blob.UploadFileAsync(AppConstants.BlobContainer, model.Picture);
				newBlobName = blobResult;
			}

			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				if (newBlobName != null)
				{
					user.Picture = newBlobName;
				}

				_mapper.Map(model, user);

				_unitOfWork.Repository<UserEntity>().UpdateAsync(user);
				var result = await _unitOfWork.SaveChangesAsync();
				if (!result)
				{
					throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)));
				}

				_logger.LogInformation(LogMessageBase.Updated, nameof(User), id);
			});

			if (!string.IsNullOrEmpty(oldBlobName))
			{
				await _blob.DeleteFileByUrlAsync(oldBlobName);
			}

			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(User)));
		}
		catch (Exception ex)
		{
			if (newBlobName != null)
			{
				await _blob.DeleteFileByUrlAsync(newBlobName);
			}

			_logger.LogError(ex, LogMessageBase.OperationFailed, nameof(User), id);
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)) + ": " + ex.Message);
		}
	}

	public async Task<List<User>> GetUsersWithPagingAsync()
	{
		var lists = await _unitOfWork.Repository<UserEntity>().GetAllAsync();
		return _mapper.Map<List<User>>(lists);
	}
	public async Task<UserInfoModel> GetByIdAsync(Guid id)
	{
		var result = await _repository.GetUserByIdAsync(id);
		return _mapper.Map<UserInfoModel>(result);
	}
	public async Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id)
	{
		var user = await _repository.GetByIdForRefreshTokenAsync(id);
		if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
			throw new TokenInvalidException("Refresh token expired");
		return user;
	}
	public async Task<User> GetByRefreshTokenAsync(string refreshToken)
	{
		var user = await _unitOfWork.Repository<UserEntity>()
									.GetByConditionAsync(x => x.RefreshToken == refreshToken & x.RefreshTokenExpiryTime > DateTime.UtcNow);

		if (user == null)
		{
			return null!;
		}
		return _mapper.Map<User>(user);
	}
	public async Task<QueryResult<User>> GetUsersWithPagingAsync(QueryInfo queryInfo)
	 => await _repository.GetUsersWithPagingAsync(queryInfo);

	public async Task<User> LoginAsync(LoginModel model)
	{
		var user = await _unitOfWork.Repository<UserEntity>().GetByConditionAsync(x => x.Email == model.Email || x.Name == model.Email);
		if (user == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), model.Email ?? ""));

		if (user.IsDeleted ?? true)
			throw new Exception("User was blocked");

		bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
		if (!isPasswordValid)
		{
			throw new BadRequestException(ErrorMessageBase.Format(ErrorMessageBase.InvalidFormat, nameof(user.Password)));
		}

		var result = _mapper.Map<User>(user);
		return result;
	}

	public async Task UpdateRefreshTokenAsync(Guid id, string refreshToken)
	{
		var user = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(id);
		if (user == null)
		{
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));
		}
		if (user.RefreshToken is null)
		{
			user.RefreshTokenExpiryTime = DateTime.MinValue;
			_unitOfWork.Repository<UserEntity>().UpdateAsync(user);
		}
		user.RefreshToken = refreshToken;
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetJwtSetting().RefreshTokenExpiration);
		_unitOfWork.Repository<UserEntity>().UpdateAsync(user);
		await _unitOfWork.SaveChangesAsync();
	}

	public async Task<OperationResult> BlockUserAsync(BlockUserModel model)
	{
		if (!await _unitOfWork.Repository<UserEntity>().CheckExistAsync(x => x.Id == model.BlockedUserId))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(UserEntity), model.BlockedUserId));

		if (await _unitOfWork.Repository<UserBlockEntity>().CheckExistAsync(x => x.BlockedByUserId == model.BlockedByUserId &&
																				  x.BlockedUserId == model.BlockedUserId))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(UserBlockEntity), model.BlockedUserId));

		var userBlockEntity = _mapper.Map<UserBlockEntity>(model);
		await _unitOfWork.Repository<UserBlockEntity>().AddAsync(userBlockEntity);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(UserBlockEntity)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(UserBlockEntity)));
	}

	public async Task<OperationResult> UnblockUserAsync(BlockUserModel model)
	{
		var entity = await _unitOfWork.Repository<UserBlockEntity>()
		   .GetByConditionAsync(x => x.BlockedByUserId == model.BlockedByUserId && x.BlockedUserId == model.BlockedUserId);
		if (entity == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(UserBlockEntity), model.BlockedUserId));

		await _unitOfWork.Repository<UserBlockEntity>().DeleteByIdAsync(entity.Id);
		if (!await _unitOfWork.SaveChangesAsync())
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(UserBlockEntity)));
		}
		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(UserBlockEntity)));
	}

	public async Task<OperationResult> BanOrUnbanAsync(BanUserModel model)
	{
		var user = await _unitOfWork.Repository<UserEntity>().GetByConditionAsync(x => x.Id == model.UserId);
		if (user == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), model.UserId));
		try
		{
			// true -> false -> true
			if (model.IsDeleted == false)
			{
				user.IsDeleted = false;
				_unitOfWork.Repository<UserEntity>().UpdateAsync(user);
				await _unitOfWork.SaveChangesAsync();
				return OperationResult.SuccessResult("Unban user successful");
			}

			user.IsDeleted = true;
			_unitOfWork.Repository<UserEntity>().UpdateAsync(user);
			await _unitOfWork.SaveChangesAsync();
			return OperationResult.SuccessResult("Ban user successful");
		}
		catch (Exception ex)
		{
			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)) + ": " + ex.Message);
		}
	}
	public LevelType MapBandToLevel(double band)
	{
		if (band < 3.0) return LevelType.A1;          // 1.0–2.5
		if (band < 4.0) return LevelType.A2;          // 3.0–3.5
		if (band < 5.0) return LevelType.B1;          // 4.0–4.5
		if (band < 6.5) return LevelType.B2;          // 5.0–6.0
		if (band < 7.5) return LevelType.C1;          // 6.5–7.0
		return LevelType.C2;                          // 7.5–9.0
	}
}
