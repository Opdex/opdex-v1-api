using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.LiquidityPools.Snapshots;

public class RetrieveLiquidityAmountInQuoteQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RetrieveLiquidityAmountInQuoteQueryHandler _handler;

    public RetrieveLiquidityAmountInQuoteQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new RetrieveLiquidityAmountInQuoteQueryHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_CallCirrusGetOpdexLiquidityPoolReservesQuery_Send()
    {
        // Arrange
        var request = new RetrieveLiquidityAmountInQuoteQuery(500000000, Address.Cirrus, new Address("tPXUEzDyZDrR8YzQ6LiAJWhVuAKB8RUjyt"),
                                                              new Address("tUHwBBmhHbaBA49hVhuVNUDmreGjSFceuD"));
        var cancellationToken = new CancellationTokenSource().Token;

        var reserves = new ReservesReceipt(50000, 888888888);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reserves);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetOpdexLiquidityPoolReservesQuery>(query => query.Address == request.Pool), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_CallCirrusGetLiquidityAmountInQuoteQuery_SendForCrs()
    {
        // Arrange
        var request = new RetrieveLiquidityAmountInQuoteQuery(500000000, Address.Cirrus, new Address("tPXUEzDyZDrR8YzQ6LiAJWhVuAKB8RUjyt"),
                                                              new Address("tUHwBBmhHbaBA49hVhuVNUDmreGjSFceuD"));
        var cancellationToken = new CancellationTokenSource().Token;

        var reserves = new ReservesReceipt(50000, 888888888);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reserves);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetLiquidityAmountInQuoteQuery>(query => query.Router == request.Router
                                                                                                            && query.AmountA == request.AmountIn
                                                                                                            && query.ReserveA == reserves.Crs
                                                                                                            && query.ReserveB == reserves.Src), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_CallCirrusGetLiquidityAmountInQuoteQuery_SendForSrc()
    {
        // Arrange
        var request = new RetrieveLiquidityAmountInQuoteQuery(89829482, new Address("tMCT728cPmhexrrqqkErDbYAC9eA1wNGZA"),
                                                              new Address("tPXUEzDyZDrR8YzQ6LiAJWhVuAKB8RUjyt"),
                                                              new Address("tUHwBBmhHbaBA49hVhuVNUDmreGjSFceuD"));
        var cancellationToken = new CancellationTokenSource().Token;

        var reserves = new ReservesReceipt(50000, 888888888);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetOpdexLiquidityPoolReservesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reserves);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetLiquidityAmountInQuoteQuery>(query => query.Router == request.Router
                                                                                                            && query.AmountA == request.AmountIn
                                                                                                            && query.ReserveA == reserves.Src
                                                                                                            && query.ReserveB == reserves.Crs), cancellationToken), Times.Once);
    }
}