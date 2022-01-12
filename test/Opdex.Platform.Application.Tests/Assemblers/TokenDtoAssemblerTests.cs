using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers;

public class TokenDtoAssemblerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TokenDtoAssembler _assembler;

    public TokenDtoAssemblerTests()
    {
        _mediatorMock = new Mock<IMediator>();

        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();

        _assembler = new TokenDtoAssembler(_mediatorMock.Object, mapper);
    }

    [Fact]
    public async Task Assemble_RetrieveTokenSummaryByMarketAndTokenIdQuery_Send()
    {
        // Arrange
        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);

        // Act
        await _assembler.Assemble(token);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSummaryByMarketAndTokenIdQuery>(query => query.MarketId == UInt256.Zero &&
                                                                                                               query.TokenId == token.Id),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Assemble_HappyPath_Map()
    {
        // Arrange
        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        var tokenSummary = new TokenSummary(1, 0, token.Id, 1.12m, 3.45m, 9, 10);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSummaryByMarketAndTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);

        // Act
        var tokenDto = await _assembler.Assemble(token);

        // Assert
        tokenDto.Id.Should().Be(token.Id);
        tokenDto.Address.Should().Be(token.Address);
        tokenDto.Name.Should().Be(token.Name);
        tokenDto.Symbol.Should().Be(token.Symbol);
        tokenDto.Decimals.Should().Be(token.Decimals);
        tokenDto.Sats.Should().Be(token.Sats);
        tokenDto.TotalSupply.Should().Be(token.TotalSupply.ToDecimal(token.Decimals));
        tokenDto.Summary.PriceUsd.Should().Be(tokenSummary.PriceUsd);
        tokenDto.Summary.DailyPriceChangePercent.Should().Be(tokenSummary.DailyPriceChangePercent);
        tokenDto.Summary.ModifiedBlock.Should().Be(tokenSummary.ModifiedBlock);
    }
}
