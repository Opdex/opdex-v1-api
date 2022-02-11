using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets;

public class SelectMarketByAddressQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectMarketByAddressQueryHandler _handler;

    public SelectMarketByAddressQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectMarketByAddressQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectMarketByAddressQuery("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectMarketByAddress_Success()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var expectedEntity = new MarketEntity
        {
            Id = 123454,
            Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            Owner = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT",
            AuthPoolCreators = false,
            AuthProviders = true,
            AuthTraders = true,
            DeployerId = 4,
            MarketFeeEnabled = true,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectMarketByAddressQuery(address);

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
    public async Task SelectMarketByAddress_Throws_NotFoundException()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var command = new SelectMarketByAddressQuery(address);

        _dbContext.Setup(db => db.ExecuteFindAsync<MarketEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MarketEntity>(null));

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Market)} not found.");
    }

    [Fact]
    public async Task SelectMarketByAddress_ReturnsNull()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        const bool findOrThrow = false;

        var command = new SelectMarketByAddressQuery(address, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<MarketEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MarketEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
