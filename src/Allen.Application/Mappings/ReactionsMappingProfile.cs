namespace Allen.Application;

public class ReactionsMappingProfile : Profile
{
    public ReactionsMappingProfile()
    {
        CreateMap<ReactionEntity, Reaction>()
            .ForMember(dest => dest.ObjectType, opt => opt.MapFrom(src => src.ObjectType.ToString()))
            .ForMember(dest => dest.ReactionType, opt => opt.MapFrom(src => src.ReactionType.ToString()));

        CreateMap<CreateOrUpdateReactionModel, ReactionEntity>()
            .ForMember(dest => dest.ReactionType,
                       opt => opt.MapFrom(src => Enum.Parse<ReactionType>(src.ReactionType!, true)));
    }
}
