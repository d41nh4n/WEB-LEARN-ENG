namespace Allen.Application;

public class MediasMappingProfile : Profile
{
    public MediasMappingProfile()
    {
        //MEDIA 
        CreateMap<CreateMediaModel, MediaEntity>()
                .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => Enum.Parse<MediaType>(src.MediaType!)))
                .ReverseMap();
        CreateMap<CreateMediaWithoutTranscriptModel, MediaEntity>().ReverseMap();

        CreateMap<UpdateMediaModel, MediaEntity>()
                .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => Enum.Parse<MediaType>(src.MediaType!)))
                .ReverseMap();

        // TRANSCRIPT
        CreateMap<CreateTranscriptModel, TranscriptEntity>().ReverseMap();
        CreateMap<UpdateTranscriptModel, TranscriptEntity>().ReverseMap();
    }
}
