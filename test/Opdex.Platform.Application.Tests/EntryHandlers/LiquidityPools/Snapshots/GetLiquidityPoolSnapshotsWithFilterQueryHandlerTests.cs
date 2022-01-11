using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.LiquidityPools.Snapshots;

public class GetLiquidityPoolSnapshotsWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<IList<LiquidityPoolSnapshot>, IEnumerable<LiquidityPoolSnapshotDto>>> _assemblerMock;

    private readonly GetLiquidityPoolSnapshotsWithFilterQueryHandler _handler;

    public GetLiquidityPoolSnapshotsWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<IList<LiquidityPoolSnapshot>, IEnumerable<LiquidityPoolSnapshotDto>>>();

        _handler = new GetLiquidityPoolSnapshotsWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object, new NullLogger<GetLiquidityPoolSnapshotsWithFilterQueryHandler>());
    }

    [Fact]
    public async Task Handle_RetrieveLiquidityPoolByAddressQuery_Send()
    {
        // Arrange
        var liquidityPool = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPool, cursor);

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(query => query.Address == liquidityPool
                                                                                                       && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveLiquidityPoolSnapshotsWithFilterQuery_Send()
    {
        // Arrange
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var cancellationToken = new CancellationTokenSource().Token;

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(query => query.LiquidityPoolId == liquidityPool.Id &&
                                                                                                                 query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_SnapshotsRetrieved_AssembleResults()
    {
        // Arrange
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 10, PagingDirection.Forward, default);
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(3, 1, 4, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 3, PagingDirection.Forward, (DateTime.UtcNow, 10));
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(3, 1, 4, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now)
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync(new LiquidityPoolSnapshotDto[snapshots.Length]);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Snapshots.Count().Should().Be(snapshots.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 10));
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(3, 1, 4, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync((new LiquidityPoolSnapshotDto[snapshots.Length - 1]));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 10));
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(3, 1, 4, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync(new LiquidityPoolSnapshotDto[snapshots.Length - 1]);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, default);
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(3, 1, 4, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync(Enumerable.Empty<LiquidityPoolSnapshotDto>);

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
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(3, 1, 4, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync(Enumerable.Empty<LiquidityPoolSnapshotDto>);

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
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(3, 1, 4, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T08:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync(Enumerable.Empty<LiquidityPoolSnapshotDto>);

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
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync(Enumerable.Empty<LiquidityPoolSnapshotDto>);

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
        var liquidityPoolAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
        var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
        var request = new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolAddress, cursor);

        var liquidityPool = new LiquidityPool(1, liquidityPoolAddress, "BTC-CRS", 1, 2, 3, 4, 5);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liquidityPool);

        var snapshots = new []
        {
            new LiquidityPoolSnapshot(1, 1, 2, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T10:59:59Z").ToUniversalTime(), DateTime.Now),
            new LiquidityPoolSnapshot(2, 1, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(), new CostSnapshot(), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z").ToUniversalTime(), DateTime.Parse("2021-11-06T09:59:59Z").ToUniversalTime(), DateTime.Now)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<IList<LiquidityPoolSnapshot>>())).ReturnsAsync(Enumerable.Empty<LiquidityPoolSnapshotDto>);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (snapshots[^1].StartDate, snapshots[^1].Id));
        dto.Cursor.Previous.Should().Be(null);
    }

    private static void AssertNext(CursorDto dto, (DateTime, ulong) pointer)
    {
        SnapshotCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private static void AssertPrevious(CursorDto dto, (DateTime, ulong) pointer)
    {
        SnapshotCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}
