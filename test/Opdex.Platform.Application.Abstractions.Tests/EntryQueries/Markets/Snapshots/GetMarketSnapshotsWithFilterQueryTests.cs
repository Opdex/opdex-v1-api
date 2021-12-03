using FluentAssertions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using Xunit;

namespace Opdex.Platform.Application.Abstractions.Tests.EntryQueries.Markets.Snapshots;

public class GetMarketSnapshotsWithFilterQueryTests
{
    [Fact]
    public void Create_MarketEmpty_ThrowArgumentNullException()
    {
        // Arrange
        Address market = Address.Empty;
        SnapshotCursor cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);

        // Act
        void Act() => new GetMarketSnapshotsWithFilterQuery(market, cursor);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Act);
        exception.ParamName.Should().Be("market");
    }

    [Fact]
    public void Create_CursorNull_ThrowArgumentNullException()
    {
        // Arrange
        Address market = new Address("PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZX");
        SnapshotCursor cursor = null;

        // Act
        void Act() => new GetMarketSnapshotsWithFilterQuery(market, cursor);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Act);
        exception.ParamName.Should().Be("cursor");
    }
}