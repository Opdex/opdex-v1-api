using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers
{
    public class StakingPositionDtoAssemblerTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        private readonly StakingPositionDtoAssembler _assembler;

        public StakingPositionDtoAssemblerTests()
        {
            _mediatorMock = new Mock<IMediator>();

            _assembler = new StakingPositionDtoAssembler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Assemble_RetrieveLiquidityPoolByIdQuery_Send()
        {
            // Arrange
            var addressStaking = new AddressStaking(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "50000000", 500, 505);

            // Act
            try
            {
                await _assembler.Assemble(addressStaking);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByIdQuery>(query => query.LiquidityPoolId == addressStaking.LiquidityPoolId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Assemble_RetrieveMarketByIdQuery_Send()
        {
            // Arrange
            var addressStaking = new AddressStaking(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "50000000", 500, 505);

            var liquidityPool = new LiquidityPool(10, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", 5, 15, 25, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

            // Act
            try
            {
                await _assembler.Assemble(addressStaking);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByIdQuery>(query => query.MarketId == liquidityPool.MarketId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Assemble_RetrieveTokenByIdQuery_Send()
        {
            // Arrange
            var addressStaking = new AddressStaking(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "50000000", 500, 505);

            var liquidityPool = new LiquidityPool(10, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", 5, 15, 25, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

            var market = new Market(5, "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM", 10, 50, "PCiNwuLQemjMk63A6r5mS2Ma9Kskki6HZK", false, false, false, 1, true, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

            // Act
            try
            {
                await _assembler.Assemble(addressStaking);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == market.StakingTokenId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Assemble_HappyPath_Map()
        {
            // Arrange
            var addressStaking = new AddressStaking(5, 10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "5000000000", 500, 505);

            var liquidityPool = new LiquidityPool(10, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", 5, 15, 25, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

            var market = new Market(5, "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM", 10, 50, "PCiNwuLQemjMk63A6r5mS2Ma9Kskki6HZK", false, false, false, 1, true, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

            var token = new Token(50, "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", true, "Opdex", "ODX", 8, 8, "10000000000000000000", 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            // Act
            var response = await _assembler.Assemble(addressStaking);

            // Assert
            response.Address.Should().Be(addressStaking.Owner);
            response.Amount.Should().Be("50.00000000");
            response.LiquidityPool.Should().Be(liquidityPool.Address);
            response.StakingToken.Should().Be(token.Address);
        }
    }
}
