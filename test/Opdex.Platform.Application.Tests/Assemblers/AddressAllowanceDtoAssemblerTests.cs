using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers;

public class AddressAllowanceDtoAssemblerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;

    private readonly AddressAllowanceDtoAssembler _assembler;

    public AddressAllowanceDtoAssemblerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        _assembler = new AddressAllowanceDtoAssembler(_mapperMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Assemble_HappyPath_Map()
    {
        // Arrange
        var source = new AddressAllowance(5, 5, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", new UInt256("500000000"), 5, 50);

        // Act
        try
        {
            await _assembler.Assemble(source);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<AddressAllowanceDto>(source), Times.Once);
    }

    [Fact]
    public async Task Assemble_RetrieveTokenByIdQuery_Send()
    {
        // Arrange
        var source = new AddressAllowance(5, 5, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", new UInt256("500000000"), 5, 50);

        // Act
        try
        {
            await _assembler.Assemble(source);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == source.TokenId && query.FindOrThrow),
                                                   CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Assemble_HappyPath_ReturnMapped()
    {
        // Arrange
        var dto = new AddressAllowanceDto
        {
            Owner = "PXRNXAEYkCjMJpqdgdRG4FzbguG4GcdZuN",
            Spender = "PRpStaZSj3T5zYkU4Dw9WiyB73KAHi5tRY"
        };
        var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
        var source = new AddressAllowance(5, 5, "PXRNXAEYkCjMJpqdgdRG4FzbguG4GcdZuN", "PRpStaZSj3T5zYkU4Dw9WiyB73KAHi5tRY", new UInt256("500000000"), 5, 50);

        _mapperMock.Setup(callTo => callTo.Map<AddressAllowanceDto>(It.IsAny<AddressAllowance>())).Returns(dto);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        // Act
        var response = await _assembler.Assemble(source);

        // Assert
        response.Owner.Should().Be(dto.Owner);
        response.Spender.Should().Be(dto.Spender);
        response.Allowance.Should().Be(FixedDecimal.Parse("5.00000000"));
        response.Token.Should().Be(token.Address);
    }
}