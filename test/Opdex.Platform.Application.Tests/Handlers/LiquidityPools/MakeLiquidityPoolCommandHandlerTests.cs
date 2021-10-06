using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Market;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.LiquidityPools
{
    public class MakeLiquidityPoolCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MakeLiquidityPoolCommandHandler _handler;

        public MakeLiquidityPoolCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new MakeLiquidityPoolCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public void MakeLiquidityPoolCommand_InvalidSummary_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new MakeLiquidityPoolCommand(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pool must be set.");
        }

        [Fact]
        public async Task MakeLiquidityPoolCommand_Sends_SelectMarketTokenByMarketAndTokenIdQuery()
        {
            // Arrange
            var liquidityPool = new LiquidityPool("PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", 5, 15, 25, 500);

            // Act
            try
            {
                await _handler.Handle(new MakeLiquidityPoolCommand(liquidityPool), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectMarketTokenByMarketAndTokenIdQuery>(c => c.MarketId == liquidityPool.MarketId &&
                                                                                                            c.TokenId == liquidityPool.SrcTokenId),
                                                       CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task MakeLiquidityPoolCommand_DoesNotSend_SelectMarketTokenByMarketAndTokenIdQuery()
        {
            // Arrange
            var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", 5, 15, 25, 500, 505);

            // Act
            try
            {
                await _handler.Handle(new MakeLiquidityPoolCommand(liquidityPool), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<SelectMarketTokenByMarketAndTokenIdQuery>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task MakeLiquidityPoolCommand_Sends_PersistMarketTokenCommand()
        {
            // Arrange
            var liquidityPool = new LiquidityPool("PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", 5, 15, 25, 500);

            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<SelectMarketTokenByMarketAndTokenIdQuery>(), CancellationToken.None), Times.Never);


            // Act
            try
            {
                await _handler.Handle(new MakeLiquidityPoolCommand(liquidityPool), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<PersistMarketTokenCommand>(c => c.MarketToken.MarketId == liquidityPool.MarketId &&
                                                                                             c.MarketToken.TokenId == liquidityPool.SrcTokenId),
                                                       CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task MakeLiquidityPoolCommand_Sends_PersistLiquidityPoolCommand()
        {
            // Arrange
            var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", 5, 15, 25, 500, 505);

            // Act
            try
            {
                await _handler.Handle(new MakeLiquidityPoolCommand(liquidityPool), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<PersistLiquidityPoolCommand>(c => c.Pool == liquidityPool), CancellationToken.None), Times.Once);
        }
    }
}
