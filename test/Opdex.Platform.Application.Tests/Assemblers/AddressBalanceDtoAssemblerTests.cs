using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Tests.Assemblers
{
    public class AddressBalanceDtoAssemblerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly AddressBalanceDtoAssembler _assembler;

        public AddressBalanceDtoAssemblerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _assembler = new AddressBalanceDtoAssembler(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Assemble_HappyPath_Map()
        {
            // Arrange
            var source = new AddressBalance(5, 5, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", 500000000, 5, 50);

            // Act
            try
            {
                await _assembler.Assemble(source);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<AddressBalanceDto>(source), Times.Once);
        }

        [Fact]
        public async Task Assemble_RetrieveTokenByIdQuery_Send()
        {
            // Arrange
            var source = new AddressBalance(5, 10, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", 500000000, 5, 50);

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
            var dto = new AddressBalanceDto { Address = "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcH" };
            var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var source = new AddressBalance(5, 5, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", 500000000, 5, 50);

            _mapperMock.Setup(callTo => callTo.Map<AddressBalanceDto>(It.IsAny<AddressBalance>())).Returns(dto);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            // Act
            var response = await _assembler.Assemble(source);

            // Assert
            response.Address.Should().Be(dto.Address);
            response.Balance.Should().Be(FixedDecimal.Parse("5.00000000"));
            response.Token.Should().Be(token.Address);
        }
    }
}
