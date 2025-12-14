namespace Allen.Application;

public class VocabularyMappingProfile : Profile
{
    public VocabularyMappingProfile()
    {
        // Vocabulary
        CreateMap<VocabularyEntity, VocabularyModel>()
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src => src.Level.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Level)
                        ? LevelType.A1
                        : Enum.Parse<LevelType>(src.Level, true)
                ));

        CreateMap<VocabularyEntity, CreateVocabularyModel>()
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src => src.Level.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Level)
                        ? LevelType.A1
                        : Enum.Parse<LevelType>(src.Level, true)
                ));

        CreateMap<VocabularyEntity, UpdateVocabularyModel>()
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src => src.Level.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Level)
                        ? LevelType.A1
                        : Enum.Parse<LevelType>(src.Level, true)
                ));

        // Vocabulary Meaning
        CreateMap<VocabularyMeaningEntity, VocabularyMeaningModel>()
            .ForMember(dest => dest.PartOfSpeech,
                opt => opt.MapFrom(src => src.PartOfSpeech.HasValue ? src.PartOfSpeech.Value.ToString() : null))
            .ReverseMap()
            .ForMember(dest => dest.PartOfSpeech,
                opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.PartOfSpeech)
                        ? (PartOfSpeechType?)null
                        : Enum.Parse<PartOfSpeechType>(src.PartOfSpeech, true)
                ));

        CreateMap<VocabularyMeaningEntity, CreateVocabularyMeaningModel>()
            .ForMember(dest => dest.PartOfSpeech,
                opt => opt.MapFrom(src => src.PartOfSpeech.HasValue ? src.PartOfSpeech.Value.ToString() : null))
            .ReverseMap()
            .ForMember(dest => dest.PartOfSpeech,
                opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.PartOfSpeech)
                        ? (PartOfSpeechType?)null
                        : Enum.Parse<PartOfSpeechType>(src.PartOfSpeech, true)
                ));

        CreateMap<VocabularyMeaningEntity, UpdateVocabularyMeaningModel>()
            .ForMember(dest => dest.PartOfSpeech,
                opt => opt.MapFrom(src => src.PartOfSpeech.HasValue ? src.PartOfSpeech.Value.ToString() : null))
            .ReverseMap()
            .ForMember(dest => dest.PartOfSpeech,
                opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.PartOfSpeech)
                        ? (PartOfSpeechType?)null
                        : Enum.Parse<PartOfSpeechType>(src.PartOfSpeech, true)
                ));

        // Vocabulary Tag
        CreateMap<VocabularyTagEntity, VocabularyTagModel>().ReverseMap();

        // Topic
        CreateMap<TopicEntity, VocabularyTopicModel>().ReverseMap();
        CreateMap<TopicEntity, CreateTopicModel>().ReverseMap();
        CreateMap<TopicEntity, UpdateTopicModel>().ReverseMap();

        // Tag
        CreateMap<TagEntity, TagModel>().ReverseMap();
        CreateMap<TagEntity, CreateTagModel>().ReverseMap();
        CreateMap<TagEntity, UpdateTagModel>().ReverseMap();
    }
}
