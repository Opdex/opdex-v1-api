using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.VaultGovernances;

public class MakeVaultGovernanceCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultGovernanceCommandHandler _handler;

    public MakeVaultGovernanceCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultGovernanceCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var vault = new VaultGovernance("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var request = new MakeVaultGovernanceCommand(vault, 500000);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultGovernanceCommand>(command => command.Vault == vault), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        var vault = new VaultGovernance("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var request = new MakeVaultGovernanceCommand(vault, 500000);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultGovernanceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
