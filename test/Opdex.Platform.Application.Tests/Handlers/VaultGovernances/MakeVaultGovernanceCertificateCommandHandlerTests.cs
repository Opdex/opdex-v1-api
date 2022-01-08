using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances.Certificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.VaultGovernances;

public class MakeVaultGovernanceCertificateCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultGovernanceCertificateCommandHandler _handler;

    public MakeVaultGovernanceCertificateCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultGovernanceCertificateCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var certificate = new VaultCertificate(5, 99, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultGovernanceCertificateCommand(certificate);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultGovernanceCertificateCommand>(command => command.Certificate == certificate), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        var certificate = new VaultCertificate(5, 99, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultGovernanceCertificateCommand(certificate);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultGovernanceCertificateCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
