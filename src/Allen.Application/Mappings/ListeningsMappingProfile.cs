namespace Allen.Application;

public class ListeningsMappingProfile : Profile
{
    public ListeningsMappingProfile()
    {
        CreateMap<CreateListeningForLearningModel, ListeningEntity>().ReverseMap();
        CreateMap<UpdateListeningForLearningModel, ListeningEntity>().ReverseMap();

        CreateMap<CreateListeningsForIeltsModel, ListeningEntity>().ReverseMap();
        CreateMap<CreateListeningForIeltsModel, ListeningEntity>().ReverseMap();
        CreateMap<UpdateListeningForIeltsModel, ListeningEntity>().ReverseMap();

        CreateMap<CreateListeningSectionModel, ListeningEntity>().ReverseMap();
        CreateMap<UpdateListeningSectionModel, ListeningEntity>().ReverseMap();
    }
}