using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Handlers.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Vaults;

public class MakeVaultCertificateCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultCertificateCommandHandler _handler;

    public MakeVaultCertificateCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultCertificateCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var certificate = new VaultCertificate(5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultCertificateCommand(certificate);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultCertificateCommand>(command => command.Certificate == certificate), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        var certificate = new VaultCertificate(5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultCertificateCommand(certificate);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultCertificateCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
