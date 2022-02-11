using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers;

public class MinedTokenDistributionScheduleDtoAssemblerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Token _token;

    private readonly MinedTokenDistributionScheduleDtoAssembler _assembler;

    public MinedTokenDistributionScheduleDtoAssemblerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _token = new Token(5, new Address("PR7BHjp3k3iE3h3kS7x4YLkDnA9FRTA8x7"), "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _assembler = new MinedTokenDistributionScheduleDtoAssembler(_mediatorMock.Object, _token);
    }

    [Fact]
    public async Task Assemble_RetrieveVaultByTokenId_Send()
    {
        // Arrange
        var distribution = new TokenDistribution(_token.Id, 10000000, 20000000, 0, 300, 400);

        // Act
        await _assembler.Assemble(distribution);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<RetrieveVaultByTokenIdQuery>(q => q.TokenId == _token.Id && q.FindOrThrow), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Assemble_RetrieveMiningGovernanceByTokenId_Send()
    {
        // Arrange
        var distribution = new TokenDistribution(_token.Id, 10000000, 20000000, 0, 300, 400);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Vault(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 11));

        // Act
        await _assembler.Assemble(distribution);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<RetrieveMiningGovernanceByTokenIdQuery>(q => q.TokenId == _token.Id && q.FindOrThrow), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Assemble_Map_ExpectedResult()
    {
        // Arrange
        var distribution = new TokenDistribution(_token.Id, 10000000, 20000000, 0, 300, 400);

        var vault = new Vault(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vault);
        var miningGovernance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 11);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(miningGovernance);

        // Act
        var result = await _assembler.Assemble(distribution);

        // Assert
        result.Vault.Should().Be(vault.Address);
        result.MiningGovernance.Should().Be(miningGovernance.Address);
        result.NextDistributionBlock.Should().Be(distribution.NextDistributionBlock);
        result.Previous.Vault.Should().Be(distribution.VaultDistribution.ToDecimal(_token.Decimals));
        result.Previous.MiningGovernance.Should().Be(distribution.MiningGovernanceDistribution.ToDecimal(_token.Decimals));
        result.Previous.Block.Should().Be(distribution.DistributionBlock);
    }
}
