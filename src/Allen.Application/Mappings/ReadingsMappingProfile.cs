namespace Allen.Application;

public class ReadingsMappingProfile : Profile
{
    public ReadingsMappingProfile()
    {
        CreateMap<ReadingPassageModel, ReadingPassageEntity>().ReverseMap();
        CreateMap<CreateReadingPassageModel, ReadingPassageEntity>().ReverseMap();
        CreateMap<UpdateReadingPassagesModel, ReadingPassageEntity>().ReverseMap();
        CreateMap<UpdateReadingPassageForLearningModel, ReadingPassageEntity>()
        .ForMember(dest => dest.LearningUnit, opt => opt.MapFrom(src => src.LearningUnit))
        .ForPath(dest => dest.LearningUnit.Level, opt => opt.MapFrom(src => ParseLevel(src.LearningUnit != null ? src.LearningUnit.Level : null)))
        .ReverseMap();

        CreateMap<CreateIeltsReadingPassagesModel, ReadingPassageEntity>();
        CreateMap<CreateIeltsReadingPassageModel, ReadingPassageEntity>();
        CreateMap<CreateLearningReadingPassageModel, ReadingPassageEntity>()
            .ForMember(dest => dest.Content, opt => opt.Ignore());

        CreateMap<CreateReadingParagraphModel, ReadingParagraphEntity>().ReverseMap();
        CreateMap<UpdateReadingParagraphModel, ReadingParagraphEntity>().ReverseMap();
    }
    private static LevelType ParseLevel(string? level)
    {
        if (!string.IsNullOrWhiteSpace(level) &&
            Enum.TryParse<LevelType>(level, out var result))
        {
            return result;
        }
        return LevelType.Unknown;
    }
}