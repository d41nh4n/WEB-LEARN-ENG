namespace Allen.Application;

public class LearningUnitsMappingProfile : Profile
{
	public LearningUnitsMappingProfile()
	{
		CreateMap<LearningUnitEntity, LearningUnit>().ReverseMap();
		CreateMap<CreateOrUpdateLearningUnitModel, LearningUnitEntity>()
			.ForMember(dest => dest.Level, opt => opt.MapFrom(src => Enum.Parse<LevelType>(src.Level!, true)))
			.ForMember(dest => dest.SkillType, opt => opt.MapFrom(src => Enum.Parse<SkillType>(src.SkillType!, true)))
			.ForMember(dest => dest.LearningUnitType, opt => opt.MapFrom(src => Enum.Parse<LearningUnitType>(src.LearningUnitType!, true)))
			.ReverseMap();

		CreateMap<LearningUnitEntity, CreateLearningUnitForReadingModel>().ReverseMap();
		CreateMap<LearningUnitEntity, CreateLearningUnitForSpeakingModel>().ReverseMap();
		CreateMap<LearningUnitEntity, UpdateLearningUnitForSpeakingModel>().ReverseMap();
		CreateMap<CreateLearningUnitForWritingModel, LearningUnitEntity>()
			.ForMember(dest => dest.Level, opt => opt.MapFrom(src => Enum.Parse<LevelType>(src.Level!)))
			.ReverseMap();
		CreateMap<UpdateLearningUnitForWritingModel, LearningUnitEntity>().ReverseMap();

		CreateMap<UpdateLearningForListeningModel, LearningUnitEntity>()
			.ForMember(dest => dest.Level, opt => opt.MapFrom(src => Enum.Parse<LevelType>(src.Level!)))
			.ReverseMap();

		CreateMap<UpdateLearningUnitStatusModel, LearningUnitEntity>()
			.ForMember(dest => dest.LearningUnitStatusType, opt => opt.MapFrom(src => Enum.Parse<LearningUnitStatusType>(src.LearningUnitStatusType!)))
			.ReverseMap();
	}
}