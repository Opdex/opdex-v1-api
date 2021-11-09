using FluentAssertions;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using Xunit;

namespace Opdex.Platform.Application.Abstractions.Tests.EntryQueries.LiquidityPools.Snapshots
{
    public class GetLiquidityPoolSnapshotsWithFilterQueryTests
    {
        [Fact]
        public void Create_LiquidityPoolEmpty_ThrowArgumentNullException()
        {
            // Arrange
            Address liquidityPool = Address.Empty;
            SnapshotCursor cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);

            // Act
            void Act() => new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPool, cursor);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Act);
            exception.ParamName.Should().Be("liquidityPool");
        }

        [Fact]
        public void Create_CursorNull_ThrowArgumentNullException()
        {
            // Arrange
            Address liquidityPool = new Address("PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZX");
            SnapshotCursor cursor = null;

            // Act
            void Act() => new GetLiquidityPoolSnapshotsWithFilterQuery(liquidityPool, cursor);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Act);
            exception.ParamName.Should().Be("cursor");
        }
    }
}
