namespace Allen.Application;

public class FeedbacksMappingProfile : Profile
{
	public FeedbacksMappingProfile()
	{
		CreateMap<CreateOrUpdateFeedbackModel, FeedbackEntity>().ReverseMap();
	}
}
