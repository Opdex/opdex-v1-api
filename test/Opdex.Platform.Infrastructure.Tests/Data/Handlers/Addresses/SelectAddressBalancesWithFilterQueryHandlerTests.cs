using AutoMapper;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses
{
    public class SelectAddressBalancesWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectAddressBalancesWithFilterQueryHandler _handler;

        public SelectAddressBalancesWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectAddressBalancesWithFilterQueryHandler(_dbContext.Object, mapper);
        }
    }
}
