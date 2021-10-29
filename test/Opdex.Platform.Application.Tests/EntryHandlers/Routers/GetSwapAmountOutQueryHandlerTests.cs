using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Routers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Routers
{
    public class GetSwapAmountOutQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetSwapAmountOutQueryHandler _handler;

        public GetSwapAmountOutQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetSwapAmountOutQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task SingleHop_RetrieveLiquidityPoolReserves_Once()
        {
            // Arrange
            SetupDomain();

            var request = new GetSwapAmountOutQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), Address.Cirrus,
                                                    FixedDecimal.Parse("34209.34821118"), new Address("PCiNwuLQemjMk63A6r5mS2Ma9Kskki6HZK"));
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(), cancellationToken), Times.Once);
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SingleHop_CallCirrusGetAmountOutStandardQuoteQuery_Send()
        {
            // Arrange
            SetupDomain();

            var request = new GetSwapAmountOutQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), Address.Cirrus,
                                                    FixedDecimal.Parse("34209.34821118"), new Address("PCiNwuLQemjMk63A6r5mS2Ma9Kskki6HZK"));
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetAmountOutStandardQuoteQuery>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task MultiHop_RetrieveLiquidityPoolReserves_Twice()
        {
            // Arrange
            SetupDomain();

            var request = new GetSwapAmountOutQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), new Address("PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM"),
                                                    FixedDecimal.Parse("34209.34821118"), new Address("PCiNwuLQemjMk63A6r5mS2Ma9Kskki6HZK"));
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(), cancellationToken), Times.Exactly(2));
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), cancellationToken), Times.Exactly(2));
        }

        [Fact]
        public async Task MultiHop_CallCirrusGetAmountOutMultiHopQuoteQuery_Send()
        {
            // Arrange
            SetupDomain();

            var request = new GetSwapAmountOutQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), new Address("PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM"),
                                                    FixedDecimal.Parse("34209.34821118"), new Address("PCiNwuLQemjMk63A6r5mS2Ma9Kskki6HZK"));
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetAmountOutMultiHopQuoteQuery>(), cancellationToken), Times.Once);
        }

        private void SetupDomain()
        {
            // tokens
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == Address.Cirrus), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 10, 10));
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address != Address.Cirrus), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 8, 1000000000, 21000000000000000, 50, 50));

            // market
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Market(5, new Address("PWbQLxNnYdyUBLmeEL3ET1WdNx7dvbH8mi"), 10, 20, Address.Empty,
                                                  new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"), false, false, false, 3, true, 2, 250));

            // router
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new MarketRouter(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 5, true, 2, 200));

            // pools
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new LiquidityPool(5, new Address("PPK3k64UASByx3168NU1ejXgugbW9PHRG6"), 10, 50, 5, 20, 250));

            // reserves
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Reserves(5000, 2319148201));
        }
    }
}
