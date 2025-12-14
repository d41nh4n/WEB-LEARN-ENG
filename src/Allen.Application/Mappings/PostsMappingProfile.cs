namespace Allen.Application;

public class PostsMappingProfile : Profile
{
    public PostsMappingProfile()
    {
        CreateMap<PostEntity, Post>().ReverseMap();
        CreateMap<CreatePostModel, PostEntity>().ReverseMap();
        CreateMap<PostEntity, UpdatePostModel>().ReverseMap();
    }
}
