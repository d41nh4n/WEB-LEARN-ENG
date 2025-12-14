namespace Allen.Application;

public class WritingsMappingProfile : Profile
{
    public WritingsMappingProfile()
    {
        CreateMap<WritingEntity, WritingLearningModel>().ReverseMap();
        CreateMap<CreateLearningWritingModel, WritingEntity>().ReverseMap();
        CreateMap<UpdateLearningWritingModel, WritingEntity>().ReverseMap();

        CreateMap<WritingEntity, WritingIeltsModel>().ReverseMap();
        CreateMap<CreateIeltsWritingModel, WritingEntity>().ReverseMap();
        CreateMap<UpdateIeltsWritingModel, WritingEntity>().ForMember(dest => dest.SourceUrl, opt => opt.Ignore());
    }
}