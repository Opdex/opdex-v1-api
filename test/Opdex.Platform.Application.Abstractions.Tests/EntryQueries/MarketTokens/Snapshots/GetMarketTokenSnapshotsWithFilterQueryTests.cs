using FluentAssertions;
using Opdex.Platform.Application.Abstractions.EntryQueries.MarketTokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using Xunit;

namespace Opdex.Platform.Application.Abstractions.Tests.EntryQueries.MarketTokens.Snapshots;

public class GetMarketTokenSnapshotsWithFilterQueryTests
{
    [Fact]
    public void Create_MarketEmpty_ThrowArgumentNullException()
    {
        // Arrange
        Address market = Address.Empty;
        Address token = new Address("PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZU");
        SnapshotCursor cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);

        // Act
        void Act() => new GetMarketTokenSnapshotsWithFilterQuery(market, token, cursor);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Act);
        exception.ParamName.Should().Be("market");
    }

    [Fact]
    public void Create_TokenEmpty_ThrowArgumentNullException()
    {
        // Arrange
        Address market = new Address("PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZU");
        Address token = Address.Empty;
        SnapshotCursor cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);

        // Act
        void Act() => new GetMarketTokenSnapshotsWithFilterQuery(market, token, cursor);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Act);
        exception.ParamName.Should().Be("token");
    }

    [Fact]
    public void Create_CursorNull_ThrowArgumentNullException()
    {
        // Arrange
        Address market = new Address("PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZU");
        Address token = new Address("PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZX");
        SnapshotCursor cursor = null;

        // Act
        void Act() => new GetMarketTokenSnapshotsWithFilterQuery(market, token, cursor);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Act);
        exception.ParamName.Should().Be("cursor");
    }
}