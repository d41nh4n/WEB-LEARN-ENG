namespace Allen.Infrastructure;

[RegisterService(typeof(IUsersRepository))]
public class UsersRepository(
	SqlApplicationDbContext context,
	IMapper _mapper)
	: RepositoryBase<UserEntity>(context), IUsersRepository
{
	private readonly SqlApplicationDbContext _context = context;
	public async Task<QueryResult<User>> GetUsersWithPagingAsync(QueryInfo queryInfo)
	{
		var query = _context.Users
			.AsNoTracking()
			.Where(x => EF.Functions.Collate(x.Email, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? "")
						&& x.IsDeleted == queryInfo.IsDeleted);

		var entities = await query
			.OrderByDescending(x => x.CreatedAt)
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.Select(x => new User
			{
				Id = x.Id,
				Email = x.Email,
				Name = x.Name,
				Picture = x.Picture,
				Phone = x.Phone,
				CreatedAt = x.CreatedAt,
				Role = x.UserRoles.OrderBy(ur => ur.Role!.Name).Select(ur => ur.Role!.Name.ToString()).FirstOrDefault() ?? string.Empty
			})
			.ToListAsync();

		return new QueryResult<User>
		{
			Data = entities,
			TotalCount = queryInfo.NeedTotalCount
				? await query.CountAsync()
				: 0
		};
	}

	public async Task<UserInfoModel> GetUserByIdAsync(Guid id)
	{
		return await _context.Users.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(u => new UserInfoModel
			{
				Id = u.Id,
				Email = u.Email,
				Name = u.Name,
				Picture = u.Picture,
				BirthDay = u.BirthDay,
				Phone = u.Phone,
				IsDeleted = u.IsDeleted,
				Role = u.UserRoles.OrderBy(ur => ur.Role!.Name).Select(ur => ur.Role!.Name.ToString()).FirstOrDefault() ?? string.Empty
			})
			.FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));
	}
	public async Task<UserForTokenModel?> GetUserTokenDataAsync(Guid id)
	{
		var user = await _context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(u => new UserForTokenModel
			{
				Roles = u.UserRoles.Select(ur => new Role { Name = ur.Role!.Name.ToString() }).ToList(),
				Permissions = u.UserRoles
								.SelectMany(ur => ur.Role!.RolePermissions)
								.Select(rp => new Permission
								{
									Id = rp.Permission!.Id,
									Resource = rp.Permission.Resource,
									Action = rp.Permission.Action
								})
								.Distinct()
								.ToList()
			})
			.FirstOrDefaultAsync();

		if (user == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));

		return user;
	}
	public async Task<User> GetByRefreshTokenAsync(string refreshToken)
	{
		var entities = await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiryTime > DateTime.UtcNow);

		return _mapper.Map<User>(entities);
	}
	public async Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id)
	{
		var user = await (from u in _context.Users.AsNoTracking()
						  where u.Id == id
						  select new UserForTokenModel
						  {
							  Id = u.Id,
							  Email = u.Email,
							  Name = u.Name,
							  Picture = u.Picture,
							  RefreshToken = u.RefreshToken,
							  RefreshTokenExpiryTime = u.RefreshTokenExpiryTime
						  }).FirstOrDefaultAsync();

		return user ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));
	}
	public async Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id)
	{
		var user = await _context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(u => new UserForTokenModel
			{
				Roles = u.UserRoles.Select(ur => new Role { Name = ur.Role!.Name.ToString() }).ToList(),
				Permissions = u.UserRoles
								.SelectMany(ur => ur.Role!.RolePermissions)
								.Select(rp => new Permission
								{
									Id = rp.Permission!.Id,
									Resource = rp.Permission.Resource,
									Action = rp.Permission.Action
								})
								.Distinct()
								.ToList()
			})
			.FirstOrDefaultAsync();

		if (user == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));

		return user;
	}
	public async Task<List<LearningUnitEntity>> GetUnitsForSkillAsync(SkillType skill, LevelType targetLevel, Guid userId, int limit = 10)
	{
		int currentCount = (int)(limit * 0.4);
		int aboveCount = (int)(limit * 0.4);
		int belowCount = limit - currentCount - aboveCount;

		// Lấy list các UnitId mà user đã làm
		var doneUnitIds = await _context.UserTestAttempts
			.Where(x => x.UserId == userId)
			.Select(x => x.LearningUnitId)
			.ToListAsync();

		// Query unit, loại những unit user đã làm
		var data = await _context.LearningUnits
			.Where(u => u.SkillType == skill)
			.Where(u =>
				u.Level == targetLevel ||
				u.Level == targetLevel + 1 ||
				u.Level == targetLevel - 1)
			.Where(u => !doneUnitIds.Contains(u.Id))
			.Where(u => u.LearningUnitStatusType == LearningUnitStatusType.Public)
			.Where(u => u.LearningUnitType == LearningUnitType.Ielts)
			.AsNoTracking()
			.ToListAsync();

		// Group trong memory
		var current = data
			.Where(u => u.Level == targetLevel)
			.OrderBy(x => Guid.NewGuid())
			.Take(currentCount);

		var above = data
			.Where(u => u.Level == targetLevel + 1)
			.OrderBy(x => Guid.NewGuid())
			.Take(aboveCount);

		var below = data
			.Where(u => u.Level == targetLevel - 1)
			.OrderBy(x => Guid.NewGuid())
			.Take(belowCount);

		return current
			.Concat(above)
			.Concat(below)
			.ToList();
	}
}
