using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers;

public class VaultDtoAssemblerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;

    private readonly VaultDtoAssembler _assembler;

    public VaultDtoAssemblerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();

        _assembler = new VaultDtoAssembler(_mapperMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Assemble_TokenAddress()
    {
        // Arrange
        var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var balance = new AddressBalance(5, 5, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", 500000000, 5, 50);
        _mediatorMock
            .Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(balance);

        var vault = new Vault(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);

        _mapperMock.Setup(callTo => callTo.Map<VaultDto>(vault)).Returns(new VaultDto());

        // Act
        var dto = await _assembler.Assemble(vault);

        // Assert
        dto.Token.Should().Be(token.Address);
    }

    [Fact]
    public async Task Assemble_BalanceNotNull_TokensLockedFromBalance()
    {
        // Arrange
        var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var balance = new AddressBalance(5, 5, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", 500000000, 5, 50);
        _mediatorMock
            .Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(balance);

        var vault = new Vault(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);

        _mapperMock.Setup(callTo => callTo.Map<VaultDto>(vault)).Returns(new VaultDto());

        // Act
        var dto = await _assembler.Assemble(vault);

        // Assert
        dto.TokensLocked.Should().Be(balance.Balance.ToDecimal(token.Decimals));
    }

    [Fact]
    public async Task Assemble_NullBalance_TokensLockedZero()
    {
        // Arrange
        var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        _mediatorMock
            .Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync((AddressBalance)null);

        var vault = new Vault(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);

        _mapperMock.Setup(callTo => callTo.Map<VaultDto>(vault)).Returns(new VaultDto());

        // Act
        var dto = await _assembler.Assemble(vault);

        // Assert
        dto.TokensLocked.Should().Be(UInt256.Zero.ToDecimal(token.Decimals));
    }
}
