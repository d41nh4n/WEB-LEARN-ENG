using Allen.Common.Settings.Enum;

namespace Allen.Application
{
    public class FlashCardsMappingProfile : Profile
    {
        public FlashCardsMappingProfile()
        {
            // ==================== FLASHCARD ====================

            // Entity → Model
            CreateMap<FlashCardEntity, FlashCardModel>()
                .ForMember(dest => dest.Front, opt => opt.Ignore())
                .ForMember(dest => dest.Back, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Front = JsonSerializer.Deserialize<List<FlashCardContentsModel>>(src.FrontContent) ?? new List<FlashCardContentsModel>();
                    dest.Back = JsonSerializer.Deserialize<List<FlashCardContentsModel>>(src.BackContent) ?? new List<FlashCardContentsModel>();
                });

            // Model → Entity (nếu cần reverse)
            CreateMap<FlashCardModel, FlashCardEntity>()
                .ForMember(dest => dest.FrontContent, opt => opt.Ignore())
                .ForMember(dest => dest.BackContent, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.FrontContent = JsonSerializer.Serialize(src.Front);
                    dest.BackContent = JsonSerializer.Serialize(src.Back);
                });

            // FlashCardCreateRequest → Entity
            CreateMap<FlashCardCreateRequestModel, FlashCardEntity>()
                .ForMember(dest => dest.FrontContent, opt => opt.Ignore())
                .ForMember(dest => dest.BackContent, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.FrontContent = JsonSerializer.Serialize(src.FrontContents);
                    dest.BackContent = JsonSerializer.Serialize(src.BackContents);
                    dest.Hint = src.Hint;
                    dest.PersonalNotes = src.PersonalNotes;
                });

            // FlashCardUpdateRequest → Entity
            CreateMap<FlashCardUpdateRequestModel, FlashCardEntity>()
                .ForMember(dest => dest.FrontContent, opt => opt.Ignore())
                .ForMember(dest => dest.BackContent, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                        dest.FrontContent = JsonSerializer.Serialize(src.FrontContents);
                        dest.BackContent = JsonSerializer.Serialize(src.BackContents);
                        dest.Hint = src.Hint;
                        dest.PersonalNotes = src.PersonalNotes;
                        dest.IsSuspended = src.IsSuspended!.Value;
                });

            // Entity → Model
            CreateMap<FlashCardEntity, FlashCardStudyModel>()
                .ForMember(dest => dest.Front, opt => opt.Ignore())
                .ForMember(dest => dest.Back, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Front = JsonSerializer.Deserialize<List<FlashCardContentsModel>>(src.FrontContent) ?? new List<FlashCardContentsModel>();
                    dest.Back = JsonSerializer.Deserialize<List<FlashCardContentsModel>>(src.BackContent) ?? new List<FlashCardContentsModel>();
                });

            // ==================== CARD STATE & REVIEW HISTORY ====================
            CreateMap<FlashCardStateEntity, FlashCardStateModel>().ReverseMap();
            CreateMap<ReviewFLHistoryEntity, ReviewHistoryModel>().ReverseMap();

            // ==================== DECK ====================
            CreateMap<DeckEntity, CreateDeckModel>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Level,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Level)
                            ? DeckLevel.Elementary
                            : Enum.Parse<DeckLevel>(src.Level, true)));

            CreateMap<DeckEntity, DeckModel>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Level,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Level)
                            ? DeckLevel.Elementary
                            : Enum.Parse<DeckLevel>(src.Level, true)));

            CreateMap<DeckEntity, UpdateDeckModel>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Level,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Level)
                            ? DeckLevel.Elementary
                            : Enum.Parse<DeckLevel>(src.Level, true)));
        }
    }
}
