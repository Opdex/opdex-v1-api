using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Routers;
using Opdex.Platform.Application.Handlers.Routers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Routers
{
    public class RetrieveSwapAmountInQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        private readonly RetrieveSwapAmountInQueryHandler _handler;

        public RetrieveSwapAmountInQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveSwapAmountInQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task SingleHop_RetrieveLiquidityPoolReserves_Once()
        {
            // Arrange
            SetupPools();

            var router = new MarketRouter(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 5, true, 2, 200);
            var tokenIn = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 10, 10);
            var tokenOut = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 8, 1000000000, 21000000000000000, 50, 50);

            var request = new RetrieveSwapAmountInQuery(router, tokenIn, tokenOut, 500000);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(), cancellationToken), Times.Once);
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SingleHop_CallCirrusGetAmountInStandardQuoteQuery_Send()
        {
            // Arrange
            SetupPools();

            var router = new MarketRouter(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 5, true, 2, 200);
            var tokenIn = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 10, 10);
            var tokenOut = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 8, 1000000000, 21000000000000000, 50, 50);

            var request = new RetrieveSwapAmountInQuery(router, tokenIn, tokenOut, 500000);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetAmountInStandardQuoteQuery>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task MultiHop_RetrieveLiquidityPoolReserves_Twice()
        {
            // Arrange
            SetupPools();

            var router = new MarketRouter(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 5, true, 2, 200);
            var tokenIn = new Token(5, new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), false, "Britcoin", "XGBP", 18, 1000000000000000000, UInt256.Parse("1000000000000000000000000000000000000"), 10, 10);
            var tokenOut = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 8, 1000000000, 21000000000000000, 50, 50);

            var request = new RetrieveSwapAmountInQuery(router, tokenIn, tokenOut, 500000);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(), cancellationToken), Times.Exactly(2));
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), cancellationToken), Times.Exactly(2));
        }

        [Fact]
        public async Task MultiHop_CallCirrusGetAmountInMultiHopQuoteQuery_Send()
        {
            // Arrange
            SetupPools();

            var router = new MarketRouter(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 5, true, 2, 200);
            var tokenIn = new Token(5, new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), false, "Britcoin", "XGBP", 18, 1000000000000000000, UInt256.Parse("1000000000000000000000000000000000000"), 10, 10);
            var tokenOut = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 8, 1000000000, 21000000000000000, 50, 50);

            var request = new RetrieveSwapAmountInQuery(router, tokenIn, tokenOut, 500000);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetAmountInMultiHopQuoteQuery>(), cancellationToken), Times.Once);
        }

        private void SetupPools()
        {
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new LiquidityPool(5, new Address("PPK3k64UASByx3168NU1ejXgugbW9PHRG6"), "CRS/SRC", 10, 50, 5, 20, 250));

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ReservesReceipt(5000, 2319148201));
        }
    }
}
