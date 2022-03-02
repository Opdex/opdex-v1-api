using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Cache;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers;

public class MarketTokenDtoAssemblerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly MarketTokenDtoAssembler _assembler;
    private readonly Mock<IWrappedTokenTrustValidator> _wrappedTokenTrustValidatorMock;

    public MarketTokenDtoAssemblerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _wrappedTokenTrustValidatorMock = new Mock<IWrappedTokenTrustValidator>();

        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();

        _assembler = new MarketTokenDtoAssembler(_mediatorMock.Object, mapper, _wrappedTokenTrustValidatorMock.Object);
    }

    [Fact]
    public async Task Assemble_RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery_Send()
    {
        // Arrange
        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        var market = new Market(19, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 2, 3, null, "nkfGPPGgMkN2kwXwmu3wuFYmPBWQ38k7iY", true, true, true, 3, true, 9, 10);
        var marketToken = new MarketToken(market, token);

        // Act
        try
        {
            await _assembler.Assemble(marketToken);
        }
        catch
        {
            // ignored
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(query => query.MarketId == market.Id &&
                                                                                                                     query.SrcTokenId == token.Id),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Assemble_RetrieveTokenAttributesByTokenIdQuery_Send()
    {
        // Arrange
        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        var market = new Market(19, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 2, 3, null, "nkfGPPGgMkN2kwXwmu3wuFYmPBWQ38k7iY", true, true, true, 3, true, 9, 10);
        var marketToken = new MarketToken(market, token);

        // Act
        try
        {
            await _assembler.Assemble(marketToken);
        }
        catch
        {
            // ignored
        }

        // Assert

        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenAttributesByTokenIdQuery>(query => query.TokenId == token.Id),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Assemble_RetrieveLiquidityPoolByAddressQuery_Send()
    {
        // Arrange
        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        var market = new Market(19, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 2, 3, null, "nkfGPPGgMkN2kwXwmu3wuFYmPBWQ38k7iY", true, true, true, 3, true, 9, 10);
        var marketToken = new MarketToken(market, token);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenAttributesByTokenIdQuery>(query => query.TokenId == token.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new [] { new TokenAttribute(1, 1, TokenAttributeType.Provisional)});

        // Act
        try
        {
            await _assembler.Assemble(marketToken);
        }
        catch
        {
            // ignored
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(query => query.Address == token.Address &&
                                                                                                       query.FindOrThrow == true),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Assemble_HappyPath_Map()
    {
        // Arrange
        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        token.SetSummary(new TokenSummary(5, 10, 15, -5.00m, 23.53m, 50, 50));
        var market = new Market(19, "kN2kwXwmu3wuFYmPBWQ38k7iYnkfGPPGgM", 2, 3, null, "nkfGPPGgMkN2kwXwmu3wuFYmPBWQ38k7iY", true, true, true, 3, true, 9, 10);
        var marketToken = new MarketToken(market, token);
        marketToken.SetSummary(token.Summary);
        var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "STRAX-CRS", token.Id, 15, market.Id, 500, 505);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(liquidityPool);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenAttributesByTokenIdQuery>(query => query.TokenId == token.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new [] { new TokenAttribute(1, 1, TokenAttributeType.NonProvisional)});

        // Act
        var tokenDto = await _assembler.Assemble(marketToken);

        // Assert
        tokenDto.Id.Should().Be(token.Id);
        tokenDto.Address.Should().Be(token.Address);
        tokenDto.Name.Should().Be(token.Name);
        tokenDto.Symbol.Should().Be(token.Symbol);
        tokenDto.Decimals.Should().Be(token.Decimals);
        tokenDto.Sats.Should().Be(token.Sats);
        tokenDto.TotalSupply.Should().Be(token.TotalSupply.ToDecimal(token.Decimals));
        tokenDto.Attributes.Should().BeEquivalentTo(new [] {TokenAttributeType.NonProvisional});
        tokenDto.Summary.PriceUsd.Should().Be(token.Summary.PriceUsd);
        tokenDto.Summary.DailyPriceChangePercent.Should().Be(token.Summary.DailyPriceChangePercent);
        tokenDto.Summary.CreatedBlock.Should().Be(token.Summary.CreatedBlock);
        tokenDto.Summary.ModifiedBlock.Should().Be(token.Summary.ModifiedBlock);
        tokenDto.LiquidityPool.Should().Be(liquidityPool.Address);
        tokenDto.Market.Should().Be(market.Address);
    }
}
