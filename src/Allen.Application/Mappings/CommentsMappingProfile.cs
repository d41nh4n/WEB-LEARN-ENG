namespace Allen.Application;
public class CommentsMappingProfile : Profile
{
    public CommentsMappingProfile()
    {
        CreateMap<CommentEntity, Comment>();
        CreateMap<CreateCommentModel, CommentEntity>();
        CreateMap<UpdateCommentModel, CommentEntity>();
    }
}
