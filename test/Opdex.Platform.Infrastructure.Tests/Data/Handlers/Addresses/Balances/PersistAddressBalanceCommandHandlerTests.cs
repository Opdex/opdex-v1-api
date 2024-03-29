using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses.Balances;

public class PersistAddressBalanceCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly PersistAddressBalanceCommandHandler _handler;

    public PersistAddressBalanceCommandHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        var logger = new NullLogger<PersistAddressBalanceCommandHandler>();

        _dbContext = new Mock<IDbContext>();
        _handler = new PersistAddressBalanceCommandHandler(_dbContext.Object, mapper, logger);
    }

    [Fact]
    public async Task Insert_AddressBalance_CorrectTable()
    {
        // Arrange
        var balance = new AddressBalance(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3);
        var command = new PersistAddressBalanceCommand(balance);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.StartsWith("INSERT INTO address_balance"))), Times.Once);
    }

    [Fact]
    public async Task Insert_Return_LastInsertId()
    {
        // Arrange
        var balance = new AddressBalance(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3);
        var command = new PersistAddressBalanceCommand(balance);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("SELECT LAST_INSERT_ID();"))), Times.Once);
    }

    [Fact]
    public async Task Update_AddressBalance_CorrectTable()
    {
        // Arrange
        var balance = new AddressBalance(1, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3, 3000);
        var command = new PersistAddressBalanceCommand(balance);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.StartsWith("UPDATE address_balance"))), Times.Once);
    }

    [Fact]
    public async Task Insert_AddressBalance_Success()
    {
        const ulong expectedId = 10ul;
        var balance = new AddressBalance(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3);
        var command = new PersistAddressBalanceCommand(balance);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_AddressBalance_Success()
    {
        const ulong expectedId = 10ul;
        var balance = new AddressBalance(expectedId, 1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3, 4);
        var command = new PersistAddressBalanceCommand(balance);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsAddressBalance_Fail()
    {
        const ulong expectedId = 0;
        var balance = new AddressBalance(expectedId, 1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000, 3, 4);
        var command = new PersistAddressBalanceCommand(balance);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Throws(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
