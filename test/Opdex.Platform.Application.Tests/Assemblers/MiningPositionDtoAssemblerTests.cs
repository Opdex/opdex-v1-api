using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers
{
    public class MiningPositionDtoAssemblerTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        private readonly MiningPositionDtoAssembler _assembler;

        public MiningPositionDtoAssemblerTests()
        {
            _mediatorMock = new Mock<IMediator>();

            _assembler = new MiningPositionDtoAssembler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Assemble_RetrieveMiningPoolByIdQuery_Send()
        {
            // Arrange
            var addressMining = new AddressMining(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", UInt256.Parse("50000000"), 500, 505);

            // Act
            try
            {
                await _assembler.Assemble(addressMining);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByIdQuery>(query => query.MiningPoolId == addressMining.MiningPoolId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Assemble_RetrieveLiquidityPoolByIdQuery_Send()
        {
            // Arrange
            var addressMining = new AddressMining(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", UInt256.Parse("50000000"), 500, 505);

            var miningPool = new MiningPool(10, 5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", UInt256.Parse("5000"), UInt256.Parse("1000"), 500000, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);

            // Act
            try
            {
                await _assembler.Assemble(addressMining);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByIdQuery>(query => query.LiquidityPoolId == miningPool.LiquidityPoolId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Assemble_RetrieveTokenByIdQuery_Send()
        {
            // Arrange
            var addressMining = new AddressMining(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", UInt256.Parse("50000000"), 500, 505);

            var miningPool = new MiningPool(10, 5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", UInt256.Parse("5000"), UInt256.Parse("1000"), 500000, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);

            var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "BTC-CRS", 5, 15, 25, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

            // Act
            try
            {
                await _assembler.Assemble(addressMining);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == liquidityPool.LpTokenId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Assemble_HappyPath_Map()
        {
            // Arrange
            var addressMining = new AddressMining(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", UInt256.Parse("5000000000"), 500, 505);

            var miningPool = new MiningPool(10, 5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", UInt256.Parse("5000"), UInt256.Parse("1000"), 500000, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);

            var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "BTC-CRS", 5, 15, 25, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

            var token = new Token(15, "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", true, "wBTC/CRS OLPT", "OLPT", 8, 8, UInt256.Parse("10000000000000000000"), 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            // Act
            var response = await _assembler.Assemble(addressMining);

            // Assert
            response.Address.Should().Be(addressMining.Owner);
            response.Amount.Should().Be(FixedDecimal.Parse("50.00000000"));
            response.MiningPool.Should().Be(miningPool.Address);
            response.MiningToken.Should().Be(token.Address);
        }
    }
}
