namespace Allen.Application;

public class VocabularyMappingProfile : Profile
{
    public VocabularyMappingProfile()
    {
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


        // Topic
        CreateMap<TopicEntity, VocabularyTopicModel>().ReverseMap();
        CreateMap<TopicEntity, CreateTopicModel>().ReverseMap();
        CreateMap<TopicEntity, UpdateTopicModel>().ReverseMap();


        CreateMap<VocabularyEntity, VocabularyPreviewModel>()
            .ForMember(dest => dest.Word,
                opt => opt.MapFrom(src => src.Word))

            .ForMember(dest => dest.Topic,
                opt => opt.MapFrom(src => src.Topic))

            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src => src.Level.ToString()))

            .ForMember(dest => dest.IsExisting,
                opt => opt.Ignore())

            .ForMember(dest => dest.Meanings,
                opt => opt.MapFrom(src => src.VocabularyMeanings));
    }
}
