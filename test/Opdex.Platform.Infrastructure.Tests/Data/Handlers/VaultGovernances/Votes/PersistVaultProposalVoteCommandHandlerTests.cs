using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Votes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances.Votes;

public class PersistVaultProposalVoteCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PersistVaultProposalVoteCommandHandler _handler;

    public PersistVaultProposalVoteCommandHandlerTests()
    {
        _dbContextMock = new Mock<IDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new PersistVaultProposalVoteCommandHandler(_dbContextMock.Object, Mock.Of<ILogger<PersistVaultProposalVoteCommandHandler>>(), _mapperMock.Object);
    }

    [Fact]
    public async Task Insert_VaultProposalVote_Success()
    {
        const ulong expectedId = 10ul;
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50);
        var command = new PersistVaultProposalVoteCommand(vote);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalVoteEntity>(vote)).Returns(new VaultProposalVoteEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_VaultProposalVote_Success()
    {
        const ulong expectedId = 10ul;
        var vote = new VaultProposalVote(expectedId, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50, 500);
        var command = new PersistVaultProposalVoteCommand(vote);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalVoteEntity>(vote)).Returns(new VaultProposalVoteEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsVaultProposalVote_Fail()
    {
        const ulong expectedId = 0;
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50);
        var command = new PersistVaultProposalVoteCommand(vote);

        _mapperMock.Setup(callTo => callTo.Map<VaultProposalVoteEntity>(vote)).Returns(new VaultProposalVoteEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
