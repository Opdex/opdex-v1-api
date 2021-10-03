using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.MiningPools
{
    public class MiningPoolsCursorTests
    {
        [Fact]
        public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Any, SortDirectionType.ASC, 50 + 1, PagingDirection.Forward, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>("limit", Act);
        }

        [Theory]
        [ClassData(typeof(InvalidLongPointerData))]
        public void Create_InvalidPointer_ThrowArgumentException(PagingDirection pagingDirection, ulong pointer)
        {
            // Arrange
            // Act
            void Act() => new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Any, SortDirectionType.ASC, 25, pagingDirection, pointer);

            // Assert
            Assert.Throws<ArgumentException>("pointer", Act);
        }

        [Fact]
        public void Create_NullLiquidityPoolsProvided_SetToEmpty()
        {
            // Arrange
            // Act
            var cursor = new MiningPoolsCursor(null, MiningStatusFilter.Any, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Assert
            cursor.LiquidityPools.Should().BeEmpty();
        }

        [Fact]
        public void ToString_StringifiesCursor_FormatCorrectly()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(new Address[] { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", "P8bB9yPr3vVByqfmM5KXftyGckAtAdu6f8" }, MiningStatusFilter.Active, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.ToString();

            // Assert
            result.Should().Contain("liquidityPools:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;");
            result.Should().Contain("liquidityPools:P8bB9yPr3vVByqfmM5KXftyGckAtAdu6f8;");
            result.Should().Contain("miningStatus:Active");
            result.Should().Contain("direction:ASC;");
            result.Should().Contain("limit:25;");
            result.Should().Contain("paging:Forward;");
            result.Should().Contain("pointer:NTAw;");
        }

        [Fact]
        public void Turn_NonIdenticalPointer_ReturnAnotherCursor()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(new Address[] { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L" }, MiningStatusFilter.Any, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.Turn(PagingDirection.Backward, 567);

            // Assert
            result.Should().BeOfType<MiningPoolsCursor>();
            var adjacentCursor = (MiningPoolsCursor)result;
            adjacentCursor.LiquidityPools.Should().BeEquivalentTo(cursor.LiquidityPools);
            adjacentCursor.MiningStatus.Should().Be(cursor.MiningStatus);
            adjacentCursor.SortDirection.Should().Be(cursor.SortDirection);
            adjacentCursor.Limit.Should().Be(cursor.Limit);
            adjacentCursor.PagingDirection.Should().Be(PagingDirection.Backward);
            adjacentCursor.Pointer.Should().Be(567);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData(";:;;;;;::;;;:::;;;:::;;:::;;")]
        [InlineData("miningStatus:Invalid;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // invalid miningStatus
        [InlineData("direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // missing includeZeroAmounts
        [InlineData("direction:Invalid;limit:50;paging:Forward;pointer:NTAw;")] // invalid orderBy
        [InlineData("limit:50;paging:Forward;pointer:NTAw;")] // missing orderBy
        [InlineData("direction:ASC;limit:51;paging:Forward;pointer:NTAw;")] // over max limit
        [InlineData("direction:ASC;paging:Forward;pointer:NTAw;")] // missing limit
        [InlineData("direction:ASC;limit:50;paging:Invalid;pointer:NTAw;")] // invalid paging direction
        [InlineData("direction:ASC;limit:50;pointer:NTAw;")] // missing paging direction
        [InlineData("direction:ASC;limit:50;paging:Forward;pointer:LTE=;")] // pointer: -1;
        [InlineData("direction:ASC;limit:50;paging:Forward;pointer:YWJj")] // pointer: abc;
        [InlineData("direction:ASC;limit:50;paging:Forward;pointer:10")] // pointer not valid base64
        [InlineData("direction:ASC;limit:50;paging:Forward;")] // pointer missing
        public void TryParse_InvalidCursor_ReturnFalse(string stringified)
        {
            // Arrange
            // Act
            var canParse = MiningPoolsCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(false);
            cursor.Should().Be(null);
        }

        [Fact]
        public void TryParse_ValidCursor_ReturnTrue()
        {
            // Arrange
            var stringified = "liquidityPools:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;miningStatus:Inactive;direction:ASC;limit:50;paging:Forward;pointer:MTA=;"; // pointer: 10;

            // Act
            var canParse = MiningPoolsCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(true);
            cursor.LiquidityPools.Should().ContainSingle(pool => pool == "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L");
            cursor.MiningStatus.Should().Be(MiningStatusFilter.Inactive);
            cursor.SortDirection.Should().Be(SortDirectionType.ASC);
            cursor.Limit.Should().Be(50);
            cursor.PagingDirection.Should().Be(PagingDirection.Forward);
            cursor.Pointer.Should().Be(10);
        }
    }
}
