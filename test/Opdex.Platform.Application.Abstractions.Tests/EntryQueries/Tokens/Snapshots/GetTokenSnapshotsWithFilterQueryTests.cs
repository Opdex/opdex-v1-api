using FluentAssertions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using Xunit;

namespace Opdex.Platform.Application.Abstractions.Tests.EntryQueries.Tokens.Snapshots;

public class GetTokenSnapshotsWithFilterQueryTests
{
    [Fact]
    public void Create_TokenEmpty_ThrowArgumentNullException()
    {
        // Arrange
        Address token = Address.Empty;
        SnapshotCursor cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);

        // Act
        void Act() => new GetTokenSnapshotsWithFilterQuery(token, cursor);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Act);
        exception.ParamName.Should().Be("token");
    }

    [Fact]
    public void Create_CursorNull_ThrowArgumentNullException()
    {
        // Arrange
        Address token = new Address("PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZX");
        SnapshotCursor cursor = null;

        // Act
        void Act() => new GetTokenSnapshotsWithFilterQuery(token, cursor);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Act);
        exception.ParamName.Should().Be("cursor");
    }
}