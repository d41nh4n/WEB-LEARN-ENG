namespace Allen.Application;

public class TopicsMappingProfile : Profile
{
	public TopicsMappingProfile()
	{
		CreateMap<QueryResult<TopicEntity>, QueryResult<TopicModel>>()
	.ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));

		CreateMap<TopicEntity, TopicModel>().ReverseMap();
		CreateMap<TopicEntity, CreateOrUpdateTopicModel>().ReverseMap();
	}
}
