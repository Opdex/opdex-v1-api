using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances.Votes;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.VaultGovernances;

public class MakeVaultProposalVoteCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultProposalVoteCommandHandler _handler;

    public MakeVaultProposalVoteCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultProposalVoteCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50);
        var request = new MakeVaultProposalVoteCommand(vote);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultProposalVoteCommand>(command => command.Vote == vote), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50);
        var request = new MakeVaultProposalVoteCommand(vote);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultProposalVoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
