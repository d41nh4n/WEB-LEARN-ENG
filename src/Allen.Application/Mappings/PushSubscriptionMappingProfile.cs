namespace Allen.Application;
public class PushSubscriptionMappingProfile : Profile
{
    public PushSubscriptionMappingProfile()
    {
        CreateMap<PushSubscriptionEntity, SubscriptionModel>().ReverseMap();
    }
}
