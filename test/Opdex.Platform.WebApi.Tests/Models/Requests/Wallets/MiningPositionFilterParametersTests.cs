using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Wallets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Models.Requests.Wallets;

public class MiningPositionFilterParametersTests
{
    [Fact]
    public void DefaultPropertyValues()
    {
        // Arrange
        // Act
        var filters = new MiningPositionFilterParameters();

        // Assert
        filters.MiningPools.Should().BeEmpty();
        filters.LiquidityPools.Should().BeEmpty();
        filters.IncludeZeroAmounts.Should().Be(false);
        filters.Direction.Should().Be(default(SortDirectionType));
        filters.Limit.Should().Be(default);
    }

    [Fact]
    public void BuildCursor_CursorStringNotProvided_ReturnFiltered()
    {
        // Arrange
        var filters = new MiningPositionFilterParameters
        {
            MiningPools = new Address[] { new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm") },
            LiquidityPools = new Address[] { new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh") },
            IncludeZeroAmounts = true,
            Limit = 20,
            Direction = SortDirectionType.DESC
        };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.MiningPools.Should().BeEquivalentTo(filters.MiningPools);
        cursor.LiquidityPools.Should().BeEquivalentTo(filters.LiquidityPools);
        cursor.IncludeZeroAmounts.Should().Be(filters.IncludeZeroAmounts);
        cursor.SortDirection.Should().Be(filters.Direction);
        cursor.Limit.Should().Be(filters.Limit);
        cursor.PagingDirection.Should().Be(PagingDirection.Forward);
        cursor.Pointer.Should().Be(default);
        cursor.IsFirstRequest.Should().Be(true);
    }

    [Fact]
    public void BuildCursor_NotABase64CursorString_ReturnNull()
    {
        // Arrange
        var filters = new MiningPositionFilterParameters { EncodedCursor = "NOT_BASE_64_****" };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.Should().Be(null);
    }

    [Fact]
    public void BuildCursor_NotAValidCursorString_ReturnNull()
    {
        // Arrange
        var filters = new MiningPositionFilterParameters { EncodedCursor = "Tk9UX1ZBTElE" };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.Should().Be(null);
    }

    [Fact]
    public void BuildCursor_ValidCursorString_ReturnCursor()
    {
        // Arrange
        var filters = new MiningPositionFilterParameters { EncodedCursor = "ZGlyZWN0aW9uOkRFU0M7bGltaXQ6MjtwYWdpbmc6Rm9yd2FyZDtpbmNsdWRlWmVyb0Ftb3VudHM6RmFsc2U7cG9pbnRlcjpNdz09Ow==" };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.Should().NotBe(null);
    }
}