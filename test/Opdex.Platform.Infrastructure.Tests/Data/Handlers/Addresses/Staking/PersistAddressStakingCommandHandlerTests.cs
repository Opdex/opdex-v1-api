using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses.Staking;

public class PersistAddressStakingCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly PersistAddressStakingCommandHandler _handler;

    public PersistAddressStakingCommandHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        var logger = new NullLogger<PersistAddressStakingCommandHandler>();

        _dbContext = new Mock<IDbContext>();
        _handler = new PersistAddressStakingCommandHandler(_dbContext.Object, mapper, logger);
    }

    [Fact]
    public async Task Insert_AddressStaking_CorrectTable()
    {
        // Arrange
        var position = new AddressStaking(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3);
        var command = new PersistAddressStakingCommand(position);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.StartsWith("INSERT INTO address_staking"))), Times.Once);
    }

    [Fact]
    public async Task Insert_Return_LastInsertId()
    {
        // Arrange
        var position = new AddressStaking(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3);
        var command = new PersistAddressStakingCommand(position);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("SELECT LAST_INSERT_ID();"))), Times.Once);
    }

    [Fact]
    public async Task Update_AddressStaking_CorrectTable()
    {
        // Arrange
        var position = new AddressStaking(1, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3, 3000);
        var command = new PersistAddressStakingCommand(position);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.StartsWith("UPDATE address_staking"))), Times.Once);
    }

    [Fact]
    public async Task Insert_AddressStaking_Success()
    {
        const ulong expectedId = 10ul;
        var staking = new AddressStaking(1, "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm", 100000000, 3);
        var command = new PersistAddressStakingCommand(staking);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_AddressStaking_Success()
    {
        const ulong expectedId = 10ul;
        var staking = new AddressStaking(expectedId, 1, "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm", 100000000, 3, 4);
        var command = new PersistAddressStakingCommand(staking);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsAddressStaking_Fail()
    {
        const ulong expectedId = 0;
        var staking = new AddressStaking(expectedId, 1, "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm", 100000000, 3, 4);
        var command = new PersistAddressStakingCommand(staking);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Throws(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
