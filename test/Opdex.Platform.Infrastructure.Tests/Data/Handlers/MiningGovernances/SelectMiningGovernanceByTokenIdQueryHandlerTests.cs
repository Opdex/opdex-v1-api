using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances;

public class SelectMiningGovernanceByTokenIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectMiningGovernanceByTokenIdQueryHandler _handler;

    public SelectMiningGovernanceByTokenIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectMiningGovernanceByTokenIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectMiningGovernanceByTokenIdQuery(5, false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectMiningGovernanceByTokenId_Success()
    {
        const ulong tokenId = 10;

        var expectedEntity = new MiningGovernanceEntity
        {
            Id = 123454,
            Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            TokenId = tokenId,
            NominationPeriodEnd = 999,
            MiningDuration = 1444,
            MiningPoolsFunded = 10,
            MiningPoolReward = 876543456789,
            CreatedBlock = 1
        };

        var command = new SelectMiningGovernanceByTokenIdQuery(tokenId);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedEntity);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.TokenId.Should().Be(expectedEntity.TokenId);
        result.Address.Should().Be(expectedEntity.Address);
        result.NominationPeriodEnd.Should().Be(expectedEntity.NominationPeriodEnd);
        result.MiningDuration.Should().Be(expectedEntity.MiningDuration);
        result.MiningPoolsFunded.Should().Be(expectedEntity.MiningPoolsFunded);
        result.MiningPoolReward.Should().Be(expectedEntity.MiningPoolReward);
    }

    [Fact]
    public void SelectMiningGovernanceByTokenId_Throws_NotFoundException()
    {
        var command = new SelectMiningGovernanceByTokenIdQuery(10);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync((MiningGovernanceEntity)null);

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Mining governance not found.");
    }

    [Fact]
    public async Task SelectMiningGovernanceByTokenId_ReturnsNull()
    {
        const ulong tokenId = 10;
        const bool findOrThrow = false;

        var command = new SelectMiningGovernanceByTokenIdQuery(tokenId, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync((MiningGovernanceEntity)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
