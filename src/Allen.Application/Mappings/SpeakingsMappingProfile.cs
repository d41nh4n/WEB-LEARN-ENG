namespace Allen.Application;

public class SpeakingsMappingProfile : Profile
{
    public SpeakingsMappingProfile()
    {
        CreateMap<CreateSpeakingModel, SpeakingEntity>().ReverseMap();
        CreateMap<CreateOrUpdateSpeakingIeltsModel, SpeakingEntity>().ReverseMap();
        CreateMap<UpdateSpeakingModel, SpeakingEntity>().ReverseMap();
        CreateMap<SubmitSpeakingIeltsModel, PronunciationModel>();
    }
}
