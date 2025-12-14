namespace Allen.Infrastructure;

[RegisterService(typeof(IFeedbacksRepository))]
public class FeedbacksRepository(SqlApplicationDbContext context)
	: RepositoryBase<FeedbackEntity>(context), IFeedbacksRepository
{
	public readonly SqlApplicationDbContext _context = context;
	public async Task<QueryResult<FeedbackModel>> GetFeedbacksAsync(FeedbackQuery feedbackQuery, QueryInfo queryInfo)
	{
		var query = from feedback in _context.Feedbacks.AsNoTracking()
					join user in _context.Users.AsNoTracking() on feedback.UserId equals user.Id
					join category in _context.Categories.AsNoTracking() on feedback.CategoryId equals category.Id
					where (feedback.Title == null || EF.Functions.Collate(feedback.Title, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? ""))
					&& (feedbackQuery.UserName == null || EF.Functions.Collate(user.Name, "Latin1_General_CI_AI").Contains(feedbackQuery.UserName ?? ""))
					&& (feedbackQuery.CategoryId == null || feedback.CategoryId == feedbackQuery.CategoryId)
					&& (feedbackQuery.CreateAt == null || (feedback.CreatedAt >= feedbackQuery.CreateAt && feedback.CreatedAt < feedbackQuery.CreateAt.Value.AddDays(1)))
					orderby feedback.CreatedAt descending
					select new FeedbackModel
					{
						Id = feedback.Id,
						UserId = user.Id,
						UserName = user.Name,
						CategoryId = category.Id,
						CategoryName = category.Name,
						Title = feedback.Title,
						Image = feedback.Image
					};

		var entities = await query
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.ToListAsync();

		var totalCount = 0;
		if (queryInfo.NeedTotalCount)
		{
			totalCount = await query.CountAsync();
		}
		return new QueryResult<FeedbackModel>
		{
			Data = entities,
			TotalCount = totalCount
		};
	}

	public async Task<FeedbackModel> GetFeedbackByIdAsync(Guid id)
	{
		return await (from feedback in _context.Feedbacks.AsNoTracking()
					  join user in _context.Users.AsNoTracking() on feedback.UserId equals user.Id
					  join category in _context.Categories.AsNoTracking() on feedback.CategoryId equals category.Id
					  orderby feedback.CreatedAt descending
					  select new FeedbackModel
					  {
						  Id = feedback.Id,
						  UserId = user.Id,
						  UserName = user.Name,
						  CategoryId = category.Id,
						  CategoryName = category.Name,
						  Title = feedback.Title,
						  Description = feedback.Description,
						  Image = feedback.Image
					  }).FirstOrDefaultAsync(x => x.Id == id)
					?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(FeedbackEntity), id));
	}

	public async Task<QueryResult<FeedbackModel>> GetFeedbacksOfUserAsync(Guid userId, FeedbackQuery feedbackQuery, QueryInfo queryInfo)
	{
		var query = from feedback in _context.Feedbacks.AsNoTracking()
					join user in _context.Users.AsNoTracking() on feedback.UserId equals user.Id
					join category in _context.Categories.AsNoTracking() on feedback.CategoryId equals category.Id
					where feedback.UserId == userId
					where (feedback.Title == null || EF.Functions.Collate(feedback.Title, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? ""))
					&& (feedbackQuery.UserName == null || EF.Functions.Collate(user.Name, "Latin1_General_CI_AI").Contains(feedbackQuery.UserName ?? ""))
					&& (feedbackQuery.CategoryId == null || feedback.CategoryId == feedbackQuery.CategoryId)
					&& (feedbackQuery.CreateAt == null || (feedback.CreatedAt >= feedbackQuery.CreateAt && feedback.CreatedAt < feedbackQuery.CreateAt.Value.AddDays(1)))
					orderby feedback.CreatedAt descending
					select new FeedbackModel
					{
						Id = feedback.Id,
						UserId = user.Id,
						UserName = user.Name,
						CategoryId = category.Id,
						CategoryName = category.Name,
						Title = feedback.Title,
						Image = feedback.Image
					};

		var entities = await query
			.Skip(queryInfo.Skip)
			.Take(queryInfo.Top)
			.ToListAsync();

		var totalCount = 0;
		if (queryInfo.NeedTotalCount)
		{
			totalCount = await query.CountAsync();
		}
		return new QueryResult<FeedbackModel>
		{
			Data = entities,
			TotalCount = totalCount
		};
	}

	public async Task<FeedbackModel> GetByIdOfUserAsync(Guid userId, Guid id)
	{
		return await (from feedback in _context.Feedbacks.AsNoTracking()
					  join user in _context.Users.AsNoTracking() on feedback.UserId equals user.Id
					  join category in _context.Categories.AsNoTracking() on feedback.CategoryId equals category.Id
					  where feedback.UserId == userId
					  orderby feedback.CreatedAt descending
					  select new FeedbackModel
					  {
						  Id = feedback.Id,
						  UserId = user.Id,
						  UserName = user.Name,
						  CategoryId = category.Id,
						  CategoryName = category.Name,
						  Title = feedback.Title,
						  Image = feedback.Image
					  }).FirstOrDefaultAsync(x => x.Id == id)
					?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(FeedbackEntity), id));
	}
}
