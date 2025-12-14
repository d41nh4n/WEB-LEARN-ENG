namespace Allen.Application;

public class PackageMappingProfile : Profile
{
    public PackageMappingProfile()
    {
        CreateMap<PackageEntity, PackageModel>().ReverseMap();
        CreateMap<PackageEntity, CreatePackageModel>().ReverseMap();
        CreateMap<PackageEntity, UpdatePackageModel>().ReverseMap();
    }
}
