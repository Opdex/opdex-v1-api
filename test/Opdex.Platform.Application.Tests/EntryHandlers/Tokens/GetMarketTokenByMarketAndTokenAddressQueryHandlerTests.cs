using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MarketTokens;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens;

public class GetMarketTokenByMarketAndTokenAddressQueryHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IModelAssembler<MarketToken, MarketTokenDto>> _assembler;
    private readonly GetMarketTokenByMarketAndTokenAddressQueryHandler _handler;

    public GetMarketTokenByMarketAndTokenAddressQueryHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _assembler = new Mock<IModelAssembler<MarketToken, MarketTokenDto>>();
        _handler = new GetMarketTokenByMarketAndTokenAddressQueryHandler(_mediator.Object, _assembler.Object);
    }

    [Fact]
    public void GetMarketTokenByMarketAndTokenAddressQuery_InvalidMarketAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address marketAddress = Address.Empty;
        Address tokenAddress = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";

        // Act
        void Act() => new GetMarketTokenByMarketAndTokenAddressQuery(marketAddress, tokenAddress);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Market address must be provided");
    }

    [Fact]
    public void GetMarketTokenByMarketAndTokenAddressQuery_InvalidTokenAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address marketAddress = "PGgMkN2kwXwmu3wuFYmBWQ38k7iYnkfGPP";
        Address tokenAddress = Address.Empty;

        // Act
        void Act() => new GetMarketTokenByMarketAndTokenAddressQuery(marketAddress, tokenAddress);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided");
    }

    [Fact]
    public async Task GetMarketTokenByMarketAndTokenAddressQuery_Sends_RetrieveMarketByAddressQuery()
    {
        // Arrange
        Address marketAddress = "PGgMkN2kwXwmu3wuFYmBWQ38k7iYnkfGPP";
        Address tokenAddress = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";

        // Act
        try
        {
            await _handler.Handle(new GetMarketTokenByMarketAndTokenAddressQuery(marketAddress, tokenAddress), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == marketAddress &&
                                                                                            query.FindOrThrow == true),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMarketTokenByMarketAndTokenAddressQuery_Sends_RetrieveTokenByAddressQuery()
    {
        // Arrange
        Address marketAddress = "PGgMkN2kwXwmu3wuFYmBWQ38k7iYnkfGPP";
        Address tokenAddress = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";
        var market = new Market(19, "kN2kwXwmu3wuFYmPBWQ38k7iYnkfGPPGgM", 2, 3, null, "nkfGPPGgMkN2kwXwmu3wuFYmPBWQ38k7iY", true, true, true, 3, true, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        // Act
        try
        {
            await _handler.Handle(new GetMarketTokenByMarketAndTokenAddressQuery(marketAddress, tokenAddress), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == tokenAddress &&
                                                                                           query.FindOrThrow == true),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMarketTokenByMarketAndTokenAddressQuery_Sends_AssembleMarketTokenDto()
    {
        // Arrange
        Address marketAddress = "PGgMkN2kwXwmu3wuFYmBWQ38k7iYnkfGPP";
        Address tokenAddress = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";
        var token = new Token(1, tokenAddress, "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        var market = new Market(19, "kN2kwXwmu3wuFYmPBWQ38k7iYnkfGPPGgM", 2, 3, null, "nkfGPPGgMkN2kwXwmu3wuFYmPBWQ38k7iY", true, true, true, 3, true, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        // Act
        await _handler.Handle(new GetMarketTokenByMarketAndTokenAddressQuery(marketAddress, tokenAddress), CancellationToken.None);


        // Assert
        _assembler.Verify(callTo => callTo.Assemble(It.Is<MarketToken>(mt => mt.Address == token.Address &&
                                                                             mt.Market == market)), Times.Once);
    }
}
