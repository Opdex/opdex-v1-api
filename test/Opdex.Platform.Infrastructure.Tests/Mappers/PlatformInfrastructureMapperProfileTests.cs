using AutoMapper;

namespace Opdex.Platform.Infrastructure.Tests.Mappers;

public abstract class PlatformInfrastructureMapperProfileTests
{
    protected readonly IMapper Mapper;

    protected PlatformInfrastructureMapperProfileTests()
    {
        Mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
    }
}
