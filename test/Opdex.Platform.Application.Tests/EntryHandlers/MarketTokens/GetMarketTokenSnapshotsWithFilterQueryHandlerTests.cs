using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Application.EntryHandlers.MarketTokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MarketTokens;

public class GetMarketTokenSnapshotsWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly GetMarketTokenSnapshotsWithFilterQueryHandler _handler;

    public GetMarketTokenSnapshotsWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetMarketTokenSnapshotsWithFilterQueryHandler(_mediatorMock.Object, _mapperMock.Object, new NullLogger<GetMarketTokenSnapshotsWithFilterQueryHandler>());
    }

    [Fact]
    public async Task Handle_TokenIsCRS_ThrowInvalidDataException()
    {
        // Arrange
        var market = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var token = Address.Cirrus;
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetMarketTokenSnapshotsWithFilterQuery(market, token, cursor);

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        Task Act() => _handler.Handle(request, cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
        exception.PropertyName.Should().Be("tokenAddress");
        exception.Message.Should().Be("Market snapshot history is not collected for the base token.");
    }

    [Fact]
    public async Task Handle_RetreiveTokenByAddressQuery_Send()
    {
        // Arrange
        var market = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var token = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetMarketTokenSnapshotsWithFilterQuery(market, token, cursor);

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == token
                                                                                               && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetreiveMarketByAddressQuery_Send()
    {
        // Arrange
        var market = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var token = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetMarketTokenSnapshotsWithFilterQuery(market, token, cursor);

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == market
                                                                                                && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveTokenSnapshotsWithFilterQuery_Send()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetMarketTokenSnapshotsWithFilterQuery(tokenAddress, marketAddress, cursor);

        var cancellationToken = new CancellationTokenSource().Token;

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotsWithFilterQuery>(query => query.TokenId == token.Id
                                                                                                         && query.MarketId == market.Id
                                                                                                         && query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_SnapshotsRetrieved_MapResults()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 10, PagingDirection.Forward, default);
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);

        // Act
        var results = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>()), Times.Exactly(snapshots.Length));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 3, PagingDirection.Forward, (DateTime.UtcNow, 10));
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Snapshots.Count().Should().Be(snapshots.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 10));
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotDto>(snapshots[0]), Times.Never);
        dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 10));
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotDto>(snapshots[snapshots.Length - 1]), Times.Never);
        dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, default);
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (snapshots[^2].StartDate, snapshots[^2].Id));
        dto.Cursor.Previous.Should().Be(null);
    }

    [Fact]
    public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (snapshots[^2].StartDate, snapshots[^2].Id));
        AssertPrevious(dto.Cursor, (snapshots[0].StartDate, snapshots[0].Id));
    }

    [Fact]
    public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (snapshots[^1].StartDate, snapshots[^1].Id));
        AssertPrevious(dto.Cursor, (snapshots[1].StartDate, snapshots[1].Id));
    }

    [Fact]
    public async Task Handle_PagingForwardLastPage_ReturnCursor()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Cursor.Next.Should().Be(null);
        AssertPrevious(dto.Cursor, (snapshots[0].StartDate, snapshots[0].Id));
    }

    [Fact]
    public async Task Handle_PagingBackwardLastPage_ReturnCursor()
    {
        // Arrange
        var marketAddress = new Address("tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD");
        var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
        var request = new GetMarketTokenSnapshotsWithFilterQuery(marketAddress, tokenAddress, cursor);

        var token = new Token(5, tokenAddress, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);
        var market = new Market(10, marketAddress, 1, 5, Address.Empty, new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"), false, false, false, 3, true, 20, 25);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new TokenSnapshot[]
        {
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new TokenSnapshot(3, 5, 0, new Ohlc<decimal>(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (snapshots[^1].StartDate, snapshots[^1].Id));
        dto.Cursor.Previous.Should().Be(null);
    }

    private void AssertNext(CursorDto dto, (DateTime, ulong) pointer)
    {
        SnapshotCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private void AssertPrevious(CursorDto dto, (DateTime, ulong) pointer)
    {
        SnapshotCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}
