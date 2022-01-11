using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults;

public class PersistVaultCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PersistVaultCommandHandler _handler;

    public PersistVaultCommandHandlerTests()
    {
        _dbContextMock = new Mock<IDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new PersistVaultCommandHandler(_dbContextMock.Object, Mock.Of<ILogger<PersistVaultCommandHandler>>(), _mapperMock.Object);
    }

    [Fact]
    public async Task Insert_Vault_Success()
    {
        const ulong expectedId = 10ul;
        var vault = new Vault("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var command = new PersistVaultCommand(vault);

        _mapperMock.Setup(callTo => callTo.Map<VaultEntity>(vault)).Returns(new VaultEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_Vault_Success()
    {
        const ulong expectedId = 10ul;
        var vault = new Vault(expectedId, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        var command = new PersistVaultCommand(vault);

        _mapperMock.Setup(callTo => callTo.Map<VaultEntity>(vault)).Returns(new VaultEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsVault_Fail()
    {
        const ulong expectedId = 0;
        var vault = new Vault("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var command = new PersistVaultCommand(vault);

        _mapperMock.Setup(callTo => callTo.Map<VaultEntity>(vault)).Returns(new VaultEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
