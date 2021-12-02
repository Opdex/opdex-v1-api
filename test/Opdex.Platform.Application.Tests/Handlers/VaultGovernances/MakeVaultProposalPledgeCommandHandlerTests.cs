using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances.Pledges;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.VaultGovernances;

public class MakeVaultProposalPledgeCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultProposalPledgeCommandHandler _handler;

    public MakeVaultProposalPledgeCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultProposalPledgeCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultProposalPledgeCommand(pledge);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultProposalPledgeCommand>(command => command.Pledge == pledge), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultProposalPledgeCommand(pledge);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultProposalPledgeCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
