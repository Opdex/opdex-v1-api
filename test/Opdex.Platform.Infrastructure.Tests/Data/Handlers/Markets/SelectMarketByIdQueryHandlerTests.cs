using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets
{
    public class SelectMarketByIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMarketByIdQueryHandler _handler;

        public SelectMarketByIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMarketByIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMarketById_Success()
        {
            const long id = 99;

            var expectedEntity = new MarketEntity
            {
                Id = 123454,
                Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                Owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN",
                AuthPoolCreators = false,
                AuthProviders = true,
                AuthTraders = true,
                DeployerId = 4,
                MarketFeeEnabled = true,
                CreatedBlock = 1,
                ModifiedBlock = 2
            };

            var command = new SelectMarketByIdQuery(id);

            _dbContext.Setup(db => db.ExecuteFindAsync<MarketEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.Address.Should().Be(expectedEntity.Address);
            result.Owner.Should().Be(expectedEntity.Owner);
            result.AuthPoolCreators.Should().Be(expectedEntity.AuthPoolCreators);
            result.AuthProviders.Should().Be(expectedEntity.AuthProviders);
            result.AuthTraders.Should().Be(expectedEntity.AuthTraders);
            result.DeployerId.Should().Be(expectedEntity.DeployerId);
            result.MarketFeeEnabled.Should().Be(expectedEntity.MarketFeeEnabled);
            result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
            result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
        }

        [Fact]
        public void SelectMarketById_Throws_NotFoundException()
        {
            const long id = 99;

            var command = new SelectMarketByIdQuery(id);

            _dbContext.Setup(db => db.ExecuteFindAsync<MarketEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MarketEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(Market)} not found.");
        }

        [Fact]
        public async Task SelectMarketById_ReturnsNull()
        {
            const long id = 99;
            const bool findOrThrow = false;

            var command = new SelectMarketByIdQuery(id, findOrThrow);

            _dbContext.Setup(db => db.ExecuteFindAsync<MarketEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MarketEntity>(null));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
