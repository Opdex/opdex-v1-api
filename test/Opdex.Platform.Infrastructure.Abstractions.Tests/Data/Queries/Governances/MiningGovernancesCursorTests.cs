using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.Governances
{
    public class MiningGovernancesCursorTests
    {
        [Fact]
        public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new MiningGovernancesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, 50 + 1, PagingDirection.Forward, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>("limit", Act);
        }

        [Theory]
        [InlineData(PagingDirection.Backward, 0)] // zero indicates first request, only possible to page forward
        public void Create_InvalidPointer_ThrowArgumentException(PagingDirection pagingDirection, ulong pointer)
        {
            // Arrange
            // Act
            void Act() => new MiningGovernancesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, 25, pagingDirection, pointer);

            // Assert
            Assert.Throws<ArgumentException>("pointer", Act);
        }

        [Fact]
        public void ToString_StringifiesCursor_FormatCorrectly()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.ToString();

            // Assert
            result.Should().Contain("minedToken:PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX;");
            result.Should().Contain("direction:ASC;");
            result.Should().Contain("limit:25;");
            result.Should().Contain("paging:Forward;");
            result.Should().Contain("pointer:NTAw;");
        }

        [Fact]
        public void Turn_NonIdenticalPointer_ReturnAnotherCursor()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.Turn(PagingDirection.Backward, 567);

            // Assert
            result.Should().BeOfType<MiningGovernancesCursor>();
            var adjacentCursor = (MiningGovernancesCursor)result;
            adjacentCursor.MinedToken.Should().Be(cursor.MinedToken);
            adjacentCursor.SortDirection.Should().Be(cursor.SortDirection);
            adjacentCursor.Limit.Should().Be(cursor.Limit);
            adjacentCursor.PagingDirection.Should().Be(PagingDirection.Backward);
            adjacentCursor.Pointer.Should().Be(567);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(";:;;;;;::;;;:::;;;:::;;:::;;")]
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
            var canParse = MiningGovernancesCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(false);
            cursor.Should().Be(null);
        }

        [Fact]
        public void TryParse_ValidCursor_ReturnTrue()
        {
            // Arrange
            var stringified = "minedToken:;direction:ASC;limit:50;paging:Forward;pointer:MTA=;"; // pointer: 10;

            // Act
            var canParse = MiningGovernancesCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(true);
            cursor.MinedToken.Should().Be(Address.Empty);
            cursor.SortDirection.Should().Be(SortDirectionType.ASC);
            cursor.Limit.Should().Be(50);
            cursor.PagingDirection.Should().Be(PagingDirection.Forward);
            cursor.Pointer.Should().Be(10);
        }
    }
}
