using AutoMapper;

namespace Opdex.Platform.Infrastructure.Tests.Mappers
{
    public abstract class PlatformInfrastructureMapperProfileTests
    {
        protected readonly IMapper _mapper;

        protected PlatformInfrastructureMapperProfileTests()
        {
            _mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        }
    }
}
