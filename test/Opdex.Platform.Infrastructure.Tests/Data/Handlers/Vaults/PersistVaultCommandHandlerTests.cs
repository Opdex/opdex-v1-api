using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults;

public class PersistVaultCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly PersistVaultCommandHandler _handler;

    public PersistVaultCommandHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        var logger = new NullLogger<PersistVaultCommandHandler>();

        _dbContext = new Mock<IDbContext>();
        _handler = new PersistVaultCommandHandler(_dbContext.Object, mapper, logger);
    }

    [Fact]
    public async Task Insert_Vault_Success()
    {
        const ulong expectedId = 10ul;
        var allowance = new Vault("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 1, "PPGBccfFS1cKedqY5ZzJY7iaeEwpXHKzNb", 4);
        var command = new PersistVaultCommand(allowance);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_Vault_Success()
    {
        const ulong expectedId = 10ul;
        var allowance = new Vault(expectedId, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 1, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PPGBccfFS1cKedqY5ZzJY7iaeEwpXHKzNb", 2, 500000000, 3, 4);
        var command = new PersistVaultCommand(allowance);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsVault_Fail()
    {
        const ulong expectedId = 0;
        var allowance = new Vault("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 1, "PPGBccfFS1cKedqY5ZzJY7iaeEwpXHKzNb", 4);
        var command = new PersistVaultCommand(allowance);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}