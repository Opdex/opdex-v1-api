using AutoMapper;

namespace Opdex.Platform.Application.Tests.Mappers
{
    public abstract class PlatformApplicationMapperProfileTests
    {
        protected readonly IMapper _mapper;

        protected PlatformApplicationMapperProfileTests()
        {
            _mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();
        }
    }
}
