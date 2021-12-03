using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Pledges;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances.Pledges;

public class PersistVaultProposalPledgeCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PersistVaultProposalPledgeCommandHandler _handler;

    public PersistVaultProposalPledgeCommandHandlerTests()
    {
        _dbContextMock = new Mock<IDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new PersistVaultProposalPledgeCommandHandler(_dbContextMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Insert_VaultProposalPledge_Success()
    {
        const ulong expectedId = 10ul;
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var command = new PersistVaultProposalPledgeCommand(pledge);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalPledgeEntity>(pledge)).Returns(new VaultProposalPledgeEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_VaultProposalPledge_Success()
    {
        const ulong expectedId = 10ul;
        var pledge = new VaultProposalPledge(expectedId, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50, 500);
        var command = new PersistVaultProposalPledgeCommand(pledge);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalPledgeEntity>(pledge)).Returns(new VaultProposalPledgeEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsVaultProposalPledge_Fail()
    {
        const ulong expectedId = 0;
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var command = new PersistVaultProposalPledgeCommand(pledge);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalPledgeEntity>(pledge)).Returns(new VaultProposalPledgeEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
