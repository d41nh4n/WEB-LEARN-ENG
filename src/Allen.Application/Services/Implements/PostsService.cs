namespace Allen.Application;

[RegisterService(typeof(IPostsService))]
public class PostsService(
	IPostsRepository _repository,
	//IPicPurifyService _picPurifyService,
	IRolesService _rolesService,
	IUnitOfWork _unitOfWork,
	IMapper _mapper,
	IBlobStorageService _blob,
	INSFWDetectionService _nsfwService
	//INsfwDetector _nsfwDetector
) : IPostsService
{
    public async Task<QueryResult<Post>> GetMyPostsAsync(GetPostQuery postQuery, QueryInfo queryInfo)
	{ 
		return await _repository.GetMyPostsAsync(postQuery, queryInfo);
    }

    public async Task<QueryResult<Post>> GetPostsWithPagingAsync(GetPostQuery postQuery, QueryInfo queryInfo)
	{
		return await _repository.GetPostsWithPagingAsync(postQuery, queryInfo);
    }

	public async Task<Post> GetByIdAsync(Guid id)
	{
		var post = await _repository.GetPostByIdAsync(id);
		if (post == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Post), id));
		return post;
	}

	public async Task<OperationResult> CreateAsync(CreatePostModel model)
	{
		var newBlobNames = new List<string>();

		try
		{
			var userExists = await _unitOfWork.Repository<UserEntity>().CheckExistByIdAsync(model.UserId);
			if (!userExists)
				return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), model.UserId));

			if (model.Images != null)
			{
				foreach (var file in model.Images)
				{
					using var stream = file.OpenReadStream();

					if (await _nsfwService.IsExplicitImageAsync(stream))
						return OperationResult.Failure("Photo contains sensitive content, cannot be posted.");

					// rewind stream if you upload later
					stream.Position = 0;

					var blobResult = await _blob.UploadFileAsync(AppConstants.BlobContainer, file);
					newBlobNames.Add(blobResult);
				}
			}

			var entity = _mapper.Map<PostEntity>(model);
			if (newBlobNames.Count > 0)
			{
				entity.Medias = string.Join(",", newBlobNames);
			}

			await _unitOfWork.Repository<PostEntity>().AddAsync(entity);

			if (!await _unitOfWork.SaveChangesAsync())
			{
				foreach (var b in newBlobNames)
				{
					await _blob.DeleteFileAsync(AppConstants.BlobContainer, b);
				}

				return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Post)));
			}

			return OperationResult.SuccessResult(
				ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Post)), new { entity.Id });
		}
		catch (Exception ex)
		{
			foreach (var b in newBlobNames)
			{
				await _blob.DeleteFileAsync(AppConstants.BlobContainer, b);
			}

			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Post)) + ": " + ex.Message);
		}
	}

	public async Task<OperationResult> UpdateAsync(Guid id, UpdatePostModel model)
	{
		var newBlobNames = new List<string>();
		var oldBlobNames = new List<string>();
		var removeBlobNames = new List<string>();

		try
		{
			var entity = await _unitOfWork.Repository<PostEntity>().GetByIdAsync(id);
			if (entity == null)
				throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Post), id));

			if (entity.UserId != model.UserId)
				return OperationResult.Failure(ErrorMessageBase.Forbidden);

            if (model.Images != null)
            {
                foreach (var file in model.Images)
                {
                    using var stream = file.OpenReadStream();

                    if (await _nsfwService.IsExplicitImageAsync(stream))
                        return OperationResult.Failure("Photo contains sensitive content, cannot be posted.");

                    stream.Position = 0;

                    var blobResult = await _blob.UploadFileAsync(AppConstants.BlobContainer, file);
                    newBlobNames.Add(blobResult);
                }
            }

            // Lấy ảnh cũ từ entity
            if (entity.Medias != null)
			{
				oldBlobNames = entity.Medias.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
			}

			// Giữ/Xóa ảnh cũ khi cập nhật
			var medias = new List<string>();

			if (model.Medias == null)
			{
				removeBlobNames = oldBlobNames;
				medias = newBlobNames;
			}
			else
			{
				var keepBlobNames = model.Medias ?? new List<string>();

				// Tính danh sách cần xóa & danh sách cuối cùng
				removeBlobNames = oldBlobNames.Except(keepBlobNames, StringComparer.OrdinalIgnoreCase).ToList();
				medias = keepBlobNames.Concat(newBlobNames).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			}

			_mapper.Map(model, entity);
			entity.Medias = medias.Count > 0 ? string.Join(",", medias) : null;

			_unitOfWork.Repository<PostEntity>().UpdateAsync(entity);
			if (!await _unitOfWork.SaveChangesAsync())
			{
				throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Post)));
			}

			foreach (var b in removeBlobNames)
			{
				await _blob.DeleteFileByUrlAsync(b);
			}

			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Post)));
		}
		catch (Exception ex)
		{
			foreach (var b in newBlobNames)
			{
				await _blob.DeleteFileAsync(AppConstants.BlobContainer, b);
			}

			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Post)) + ": " + ex.Message);
		}
	}

	public async Task<OperationResult> DeleteAsync(Guid id, Guid userId)
	{
		List<string> blobNames = new();

		try
		{
			await _unitOfWork.ExecuteWithTransactionAsync(async () =>
			{
				var entity = await _unitOfWork.Repository<PostEntity>().GetByIdAsync(id);

				var roles = await _rolesService.GetRoleForUserAsync(userId);
				if (entity.UserId != userId && !roles.Any(r => r.Name == "Admin"))
					throw new ForbiddenException(ErrorMessageBase.Forbidden);

				if (!string.IsNullOrEmpty(entity.Medias))
				{
					blobNames = entity.Medias.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
				}

				// Xóa reaction của post
				var postReactions = await _unitOfWork.Repository<ReactionEntity>()
					.GetListByConditionAsync(r => r.ObjectId == id && r.ObjectType == ObjectType.Post);
				_unitOfWork.Repository<ReactionEntity>().DeleteRangeAsync(postReactions);

				// Xóa comments + reaction của comments
				var comments = await _unitOfWork.Repository<CommentEntity>()
					.GetListByConditionAsync(c => c.ObjectId == id && c.ObjectType == ObjectType.Post);

				foreach (var c in comments)
				{
					var commentReactions = await _unitOfWork.Repository<ReactionEntity>()
						.GetListByConditionAsync(r => r.ObjectId == c.Id && r.ObjectType == ObjectType.Comment);
					_unitOfWork.Repository<ReactionEntity>().DeleteRangeAsync(commentReactions);
				}

				_unitOfWork.Repository<CommentEntity>().DeleteRangeAsync(comments);
				await _unitOfWork.Repository<PostEntity>().DeleteByIdAsync(id);
			});

			foreach (var b in blobNames)
			{
				await _blob.DeleteFileByUrlAsync(b);
			}

			return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(Post)));
		}
		catch (Exception ex)
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(Post)) + ": " + ex.Message);
		}
	}
}