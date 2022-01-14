using AutoMapper;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers;

public class MarketDtoAssemblerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<Token, TokenDto>> _tokenAssemblerMock;
    private readonly Mock<IModelAssembler<MarketToken, MarketTokenDto>> _marketTokenAssemblerMock;
    private readonly MarketDtoAssembler _assembler;

    public MarketDtoAssemblerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _tokenAssemblerMock = new Mock<IModelAssembler<Token, TokenDto>>();
        _marketTokenAssemblerMock = new Mock<IModelAssembler<MarketToken, MarketTokenDto>>();

        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();

        _assembler = new MarketDtoAssembler(_mediatorMock.Object, mapper, _tokenAssemblerMock.Object, _marketTokenAssemblerMock.Object);
    }

    [Fact]
    public async Task Assemble_RetrieveMarketSummaryByMarketIdQuery_Send()
    {
        // Arrange
        var market = new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25);

        // Act
        try
        {
            await _assembler.Assemble(market);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketSummaryByMarketIdQuery>(query => query.MarketId == market.Id),
                                                   CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Assemble_RetrieveTokenByAddressQuery_CRS_Send()
    {
        // Arrange
        var market = new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25);
        var summary = new MarketSummary(10, 25);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveMarketSummaryByMarketIdQuery>(query => query.MarketId == market.Id),
                                                   CancellationToken.None)).ReturnsAsync(summary);
        // Act
        try
        {
            await _assembler.Assemble(market);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == Address.Cirrus),
                                                   CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Assemble_AssembleToken_CRS_Send()
    {
        // Arrange
        var market = new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25);
        var summary = new MarketSummary(10, 25);
        var crs = new Token(1, "CRS", false, "Cirrus", "CRS", 8, 100_000_000, new UInt256("2100000000000000"), 10, 11);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveMarketSummaryByMarketIdQuery>(query => query.MarketId == market.Id),
                                                  CancellationToken.None)).ReturnsAsync(summary);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == Address.Cirrus),
                                                   CancellationToken.None)).ReturnsAsync(crs);
        // Act
        try
        {
            await _assembler.Assemble(market);
        }
        catch { }

        // Assert
        _tokenAssemblerMock.Verify(callTo => callTo.Assemble(crs), Times.Once);
    }

    [Fact]
    public async Task Assemble_RetrieveTokenByIdQuery_StakingToken_Send()
    {
        // Arrange
        var market = new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25);
        var summary = new MarketSummary(10, 25);
        var crs = new Token(1, "CRS", false, "Cirrus", "CRS", 8, 100_000_000, new UInt256("2100000000000000"), 10, 11);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveMarketSummaryByMarketIdQuery>(query => query.MarketId == market.Id),
                                                  CancellationToken.None)).ReturnsAsync(summary);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == Address.Cirrus),
                                                  CancellationToken.None)).ReturnsAsync(crs);

        _tokenAssemblerMock.Setup(callTo => callTo.Assemble(crs)).ReturnsAsync(new TokenDto());

        // Act
        try
        {
            await _assembler.Assemble(market);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == market.StakingTokenId),
                                                   CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Assemble_AssembleMarketToken_Send()
    {
        // Arrange
        var market = new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 2, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25);
        var summary = new MarketSummary(10, 25);
        var crs = new Token(1, "CRS", false, "Cirrus", "CRS", 8, 100_000_000, new UInt256("2100000000000000"), 10, 11);
        var stakingToken = new Token(5, "t5BL2FAC3iS1PEGC43eYNvVsovkDgib1MD", false, "Opdex Token", "ODX", 8, 100_000_000, new UInt256("2100000000000000"), 10, 11);
        var marketToken = new MarketToken(market, stakingToken);
        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveMarketSummaryByMarketIdQuery>(query => query.MarketId == market.Id),
                                                  CancellationToken.None)).ReturnsAsync(summary);

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == Address.Cirrus),
                                                  CancellationToken.None)).ReturnsAsync(crs);

        _tokenAssemblerMock.Setup(callTo => callTo.Assemble(crs)).ReturnsAsync(new TokenDto());

        _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == market.StakingTokenId),
                                                   CancellationToken.None)).ReturnsAsync(stakingToken);

        // Act
        try
        {
            await _assembler.Assemble(market);
        }
        catch { }

        // Assert
        _marketTokenAssemblerMock.Verify(callTo => callTo.Assemble(It.Is<MarketToken>(t => t.Id == stakingToken.Id)), Times.Once);
    }
}
