using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Routers;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Routers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Routers;

public class GetSwapAmountInQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetSwapAmountInQueryHandler _handler;

    public GetSwapAmountInQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new GetSwapAmountInQueryHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveTokenByAddressQuery_SendForTokenIn()
    {
        // Arrange
        var tokenIn = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 6, 1000000000, 21000000000000000, 50, 50);
        var tokenOut = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 50, 50);
        SetupDomain(tokenIn, tokenOut);

        var request = new GetSwapAmountInQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), tokenIn.Address, tokenOut.Address,
                                               FixedDecimal.Parse("34209.34821118"));
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == request.TokenIn), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveTokenByAddressQuery_SendForTokenOut()
    {
        // Arrange
        var tokenIn = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 6, 1000000000, 21000000000000000, 50, 50);
        var tokenOut = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 50, 50);
        SetupDomain(tokenIn, tokenOut);

        var request = new GetSwapAmountInQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), tokenIn.Address, tokenOut.Address,
                                               FixedDecimal.Parse("34209.34821118"));
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == request.TokenOut), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveMarketByAddressQuery_Send()
    {
        // Arrange
        var tokenIn = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 6, 1000000000, 21000000000000000, 50, 50);
        var tokenOut = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 50, 50);
        SetupDomain(tokenIn, tokenOut);

        var request = new GetSwapAmountInQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), tokenIn.Address, tokenOut.Address,
                                               FixedDecimal.Parse("34209.34821118"));
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == request.Market), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveActiveMarketRouterByMarketIdQuery_Send()
    {
        // Arrange
        var market = new Market(88, new Address("PWbQLxNnYdyUBLmeEL3ET1WdNx7dvbH8mi"), 10, 20, Address.Empty,
                                new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"), false, false, false, 3, true, 2, 250);
        var tokenIn = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 6, 1000000000, 21000000000000000, 50, 50);
        var tokenOut = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 50, 50);
        SetupDomain(tokenIn, tokenOut, market);

        var request = new GetSwapAmountInQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), tokenIn.Address, tokenOut.Address,
                                               FixedDecimal.Parse("34209.34821118"));
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveActiveMarketRouterByMarketIdQuery>(query => query.MarketId == market.Id), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveSwapAmountInQuery_Send()
    {
        // Arrange
        var tokenIn = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 6, 1000000000, 21000000000000000, 50, 50);
        var tokenOut = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 50, 50);
        SetupDomain(tokenIn, tokenOut);

        var request = new GetSwapAmountInQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), tokenIn.Address, tokenOut.Address,
                                               FixedDecimal.Parse("34209.34821118"));
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveSwapAmountInQuery>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveSwapAmountInQuery_ReturnDecimal()
    {
        // Arrange
        var tokenIn = new Token(10, new Address("PBcSmxbEwFHegzPirfirViDjAedV8S2aVi"), false, "Salami", "LAMI", 6, 1000000000, 21000000000000000, 50, 50);
        var tokenOut = new Token(5, Address.Cirrus, false, "Cirrus", "CRS", 8, 1000000000, 21000000000000000, 50, 50);
        SetupDomain(tokenIn, tokenOut);

        var request = new GetSwapAmountInQuery(new Address("PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW"), tokenIn.Address, tokenOut.Address, FixedDecimal.Parse("34209.34821118"));

        UInt256 amountIn = 53405932;
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveSwapAmountInQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(amountIn);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(amountIn.ToDecimal(tokenIn.Decimals));
    }

    private void SetupDomain(Token tokenIn, Token tokenOut, Market market = null)
    {
        // tokens
        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == tokenIn.Address), It.IsAny<CancellationToken>())).ReturnsAsync(tokenIn);
        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == tokenOut.Address), It.IsAny<CancellationToken>())).ReturnsAsync(tokenOut);

        // market
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(market ?? new Market(5, new Address("PWbQLxNnYdyUBLmeEL3ET1WdNx7dvbH8mi"), 10, 20, Address.Empty,
                                               new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"), false, false, false, 3, true, 2, 250));

        // router
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MarketRouter(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 5, true, 2, 200));
    }
}