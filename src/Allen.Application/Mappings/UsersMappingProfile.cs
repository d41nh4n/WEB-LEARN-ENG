namespace Allen.Application;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<UserEntity, User>().ReverseMap();
        CreateMap<UserEntity, UserInfoModel>().ReverseMap();
        CreateMap<RegisterModel, UserEntity>().ReverseMap();
        CreateMap<UserForTokenModel, User>().ReverseMap();
        CreateMap<UpdateUserModel, UserEntity>()
                .ForMember(dest => dest.Picture, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Picture, opt => opt.Ignore());
        CreateMap<BlockUserModel, UserBlockEntity>().ReverseMap();
    }
}

