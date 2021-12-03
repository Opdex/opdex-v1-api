using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Markets.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets.Snapshots;

public class GetMarketSnapshotsWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly GetMarketSnapshotsWithFilterQueryHandler _handler;

    public GetMarketSnapshotsWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetMarketSnapshotsWithFilterQueryHandler(_mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveMarketByAddressQuery_Send()
    {
        // Arrange
        var market = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetMarketSnapshotsWithFilterQuery(market, cursor);

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
    public async Task Handle_RetrieveMarketSnapshotsWithFilterQuery_Send()
    {
        // Arrange
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var cancellationToken = new CancellationTokenSource().Token;

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketSnapshotsWithFilterQuery>(query => query.MarketId == market.Id &&
                                                                                                          query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_SnapshotsRetrieved_MapResults()
    {
        // Arrange
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 10, PagingDirection.Forward, default);
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 30m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(30m, 30m, 30m, 30m), 50m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>()), Times.Exactly(snapshots.Length));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 3, PagingDirection.Forward, (DateTime.UtcNow, 10));
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(30m, 30m, 30m, 30m), 50m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Snapshots.Count().Should().Be(snapshots.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 10));
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(30m, 30m, 30m, 30m), 50m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<MarketSnapshotDto>(snapshots[0]), Times.Never);
        dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 10));
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(30m, 30m, 30m, 30m), 50m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<MarketSnapshotDto>(snapshots[snapshots.Length - 1]), Times.Never);
        dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, default);
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(30m, 30m, 30m, 30m), 50m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

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
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(30m, 30m, 30m, 30m), 50m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

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
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(30m, 30m, 30m, 30m), 50m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

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
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

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
        var marketAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneDay, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
        var request = new GetMarketSnapshotsWithFilterQuery(marketAddress, cursor);

        var market = new Market(1, marketAddress, 2, 3, Address.Empty, "tsenHnGSo1q69CJzWGnxohmQ9RukZsB6bB", true, true, true, 3, true, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);

        var snapshots = new []
        {
            new MarketSnapshot(1, 1, new Ohlc<decimal>(10m, 10m, 10m, 10m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime()),
            new MarketSnapshot(1, 1, new Ohlc<decimal>(20m, 20m, 20m, 20m), 20m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime())
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotDto>(It.IsAny<MarketSnapshot>())).Returns(new MarketSnapshotDto());

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