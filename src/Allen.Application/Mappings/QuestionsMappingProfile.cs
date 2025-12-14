using Newtonsoft.Json;

namespace Allen;

public class QuestionsMappingProfile : Profile
{
    public QuestionsMappingProfile()
    {
        CreateMap<CreateOrUpdateQuestionModel, QuestionEntity>().ReverseMap();
        CreateMap<CreateOrUpdateQuestionForReadingModel, QuestionEntity>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options != null ? JsonConvert.SerializeObject(src.Options) : null))
            .ReverseMap();

        CreateMap<UpdateQuestionModel, QuestionEntity>()
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => Enum.Parse<QuestionType>(src.QuestionType!)))
            .ReverseMap();

        CreateMap<CreateQuestionModel, QuestionEntity>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options != null ? JsonConvert.SerializeObject(src.Options) : null))
            .ReverseMap();

        CreateMap<CreateQuestionForListeningIeltsModel, QuestionEntity>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options != null ? JsonConvert.SerializeObject(src.Options) : null))
            .ReverseMap();

        CreateMap<CreateOrUpdateQuestionForSpeakingModel, QuestionEntity>()
			.ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => Enum.Parse<QuestionType>(src.QuestionType!)))
			.ReverseMap();

		CreateMap<CreateSubQuestionModel, SubQuestionEntity>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options != null ? JsonConvert.SerializeObject(src.Options) : null))
            .ReverseMap();

        CreateMap<CreateSubQuestionForListeningIeltsModel, SubQuestionEntity>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options != null ? JsonConvert.SerializeObject(src.Options) : null))
            .ReverseMap();

        CreateMap<CreateSubQuestionForListeningIeltsModel, SubQuestionEntity>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options != null ? JsonConvert.SerializeObject(src.Options) : null))
            .ReverseMap();

        CreateMap<CreateOrUpdateQuestionForListeningModel, QuestionEntity>()
         .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => Enum.Parse<QuestionType>(src.QuestionType!)))
         .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options != null ? JsonConvert.SerializeObject(src.Options) : null))
         .ForMember(dest => dest.TableMetadata, opt => opt.MapFrom(src => src.TableMetadata != null ? JsonConvert.SerializeObject(src.TableMetadata) : null))
         .ReverseMap();
    }
}