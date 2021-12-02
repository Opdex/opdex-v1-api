using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Proposals;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances;

public class PersistVaultProposalCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PersistVaultProposalCommandHandler _handler;

    public PersistVaultProposalCommandHandlerTests()
    {
        _dbContextMock = new Mock<IDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new PersistVaultProposalCommandHandler(_dbContextMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Insert_VaultProposal_Success()
    {
        const ulong expectedId = 10ul;
        var proposal = new VaultProposal(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000, 50);
        var command = new PersistVaultProposalCommand(proposal);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalEntity>(proposal)).Returns(new VaultProposalEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_VaultProposal_Success()
    {
        const ulong expectedId = 10ul;
        var proposal = new VaultProposal(expectedId, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, 50, 500);
        var command = new PersistVaultProposalCommand(proposal);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalEntity>(proposal)).Returns(new VaultProposalEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsVaultProposal_Fail()
    {
        const ulong expectedId = 0;
        var proposal = new VaultProposal(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000, 50);
        var command = new PersistVaultProposalCommand(proposal);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalEntity>(proposal)).Returns(new VaultProposalEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
