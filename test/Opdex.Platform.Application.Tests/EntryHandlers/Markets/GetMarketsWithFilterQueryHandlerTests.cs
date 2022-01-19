using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets;

public class GetMarketsWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<Market, MarketDto>> _assemblerMock;
    private readonly GetMarketsWithFilterQueryHandler _handler;

    public GetMarketsWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<Market, MarketDto>>();

        _handler = new GetMarketsWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object, new NullLogger<GetMarketsWithFilterQueryHandler>());
    }

    [Fact]
    public void GetMarketsWithFilter_InvalidCursor_ThrowsArgumentNullException()
    {
        // Arrange
        // Act
        void Act() => new GetMarketsWithFilterQuery(null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("markets cursor must be set.");
    }

    [Fact]
    public async Task Handle_RetrieveMarketsWithFilterQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 25, PagingDirection.Forward, (FixedDecimal.Parse("50.00"), 10));

        // Act
        try
        {
            await _handler.Handle(new GetMarketsWithFilterQuery(cursor), cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketsWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_marketsRetrieved_MapResults()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 25, PagingDirection.Forward, (FixedDecimal.Parse("50.00"), 10));
        var market = new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { market });

        // Act
        try
        {
            await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(market), Times.Once);
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 4, PagingDirection.Forward, (FixedDecimal.Parse("50.00"), 10));
        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
            new Market(20, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 40, 45)
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        dto.Markets.Count().Should().Be(markets.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Backward, (FixedDecimal.Parse("50.00"), 10));
        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
            new Market(20, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 40, 45)
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        dto.Markets.Count().Should().Be(markets.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, (FixedDecimal.Parse("50.00"), 10));
        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
            new Market(20, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 40, 45)
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        dto.Markets.Count().Should().Be(markets.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, default);

        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
            new Market(20, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 40, 45)
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (FixedDecimal.Zero, markets[1].Id));
        dto.Cursor.Previous.Should().Be(null);
    }

    [Fact]
    public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, (FixedDecimal.Parse("10.12"), 2));

        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
            new Market(20, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 40, 45)
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (FixedDecimal.Zero, markets[^2].Id));
        AssertPrevious(dto.Cursor, (FixedDecimal.Zero, markets[0].Id));
    }

    [Fact]
    public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Backward, (FixedDecimal.Parse("10.12"), 2));

        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
            new Market(20, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 40, 45)
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (FixedDecimal.Zero, markets[^1].Id));
        AssertPrevious(dto.Cursor, (FixedDecimal.Zero, markets[1].Id));
    }

    [Fact]
    public async Task Handle_PagingForwardLastPage_ReturnCursor()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, (FixedDecimal.Parse("10.12"), 2));
        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        dto.Cursor.Next.Should().Be(null);
        AssertPrevious(dto.Cursor, (FixedDecimal.Zero, markets[0].Id));
    }

    [Fact]
    public async Task Handle_PagingBackwardLastPage_ReturnCursor()
    {
        // Arrange
        var cursor = new MarketsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Backward, (FixedDecimal.Parse("10.12"), 2));
        var markets = new[]
        {
            new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25),
            new Market(15, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 30, 35),
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(markets);
        int marketAssembled = 0;
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Market>())).ReturnsAsync(() =>
        {
            var marketDto = CreateMarketDto(markets[marketAssembled]);
            marketAssembled++;
            return marketDto;
        });

        // Act
        var dto = await _handler.Handle(new GetMarketsWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (FixedDecimal.Zero, markets[^1].Id));
        dto.Cursor.Previous.Should().Be(null);
    }

    private static void AssertNext(CursorDto dto, (FixedDecimal, ulong) pointer)
    {
        MarketsCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private static void AssertPrevious(CursorDto dto, (FixedDecimal, ulong) pointer)
    {
        MarketsCursor.TryParse(dto.Previous.Base64Decode(), out var previous).Should().Be(true);
        previous.PagingDirection.Should().Be(PagingDirection.Backward);
        previous.Pointer.Should().Be(pointer);
    }

    private MarketDto CreateMarketDto(Market market)
    {
        var marketDto = new MarketDto
        {
            Id = market.Id,
            Address = market.Address,
            Owner = market.Owner,
            PendingOwner = market.PendingOwner,
            AuthProviders = market.AuthProviders,
            AuthTraders = market.AuthTraders,
            AuthPoolCreators = market.AuthPoolCreators,
            TransactionFee = market.TransactionFee,
            MarketFeeEnabled = market.MarketFeeEnabled,
            Summary = new MarketSummaryDto(),
            CrsToken = new TokenDto(),
            StakingToken = new TokenDto()
        };
        return marketDto;
    }
}
