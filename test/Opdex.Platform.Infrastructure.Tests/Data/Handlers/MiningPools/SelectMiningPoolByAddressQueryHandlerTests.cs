using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningPools;

public class SelectMiningPoolByAddressQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectMiningPoolByAddressQueryHandler _handler;

    public SelectMiningPoolByAddressQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectMiningPoolByAddressQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectMiningPoolByAddress_Success()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var expectedEntity = new MiningPoolEntity
        {
            Id = 123454,
            LiquidityPoolId = 99,
            Address = address,
            MiningPeriodEndBlock = 4,
            RewardPerBlock = 10,
            RewardPerLpt = 1,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectMiningPoolByAddressQuery(address);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningPoolEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.LiquidityPoolId.Should().Be(expectedEntity.LiquidityPoolId);
        result.MiningPeriodEndBlock.Should().Be(expectedEntity.MiningPeriodEndBlock);
        result.RewardPerBlock.Should().Be(expectedEntity.RewardPerBlock);
        result.RewardPerLpt.Should().Be(expectedEntity.RewardPerLpt);
        result.Address.Should().Be(expectedEntity.Address);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectMiningPoolByAddress_Throws_NotFoundException()
    {
        Address address = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";

        var command = new SelectMiningPoolByAddressQuery(address);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningPoolEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MiningPoolEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Mining pool not found.");
    }

    [Fact]
    public async Task SelectMiningPoolByAddress_ReturnsNull()
    {
        Address address = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
        const bool findOrThrow = false;

        var command = new SelectMiningPoolByAddressQuery(address, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningPoolEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MiningPoolEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}