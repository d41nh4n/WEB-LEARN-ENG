namespace Allen.Application;

public class CategoriesMappingProfile : Profile
{
    public CategoriesMappingProfile()
    {
        CreateMap<CategoryEntity, CategoryModel>().ReverseMap();
        CreateMap<CreateOrUpdateCategoryModel, CategoryEntity>()
            .ForMember(dest => dest.SkillType, opt => opt.MapFrom(src => src.SkillType.ToString()))
            .ReverseMap();
    }
}
