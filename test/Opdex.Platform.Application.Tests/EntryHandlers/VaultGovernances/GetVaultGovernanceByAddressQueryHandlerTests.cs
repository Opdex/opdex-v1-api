using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances;

public class GetVaultGovernanceByAddressQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<VaultGovernance, VaultGovernanceDto>> _assemblerMock;

    private readonly GetVaultGovernanceByAddressQueryHandler _handler;

    public GetVaultGovernanceByAddressQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<VaultGovernance, VaultGovernanceDto>>();

        _handler = new GetVaultGovernanceByAddressQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveVaultGovernanceByAddressQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var request = new GetVaultGovernanceByAddressQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"));

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByAddressQuery>(query => query.Vault == request.Vault && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_VaultExists_Assemble()
    {
        // Arrange
        var request = new GetVaultGovernanceByAddressQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(vault), Times.Once);
    }

    [Fact]
    public async Task Handle_Assembled_Return()
    {
        // Arrange
        var request = new GetVaultGovernanceByAddressQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var dto = new VaultGovernanceDto();
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(dto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(dto);
    }
}
