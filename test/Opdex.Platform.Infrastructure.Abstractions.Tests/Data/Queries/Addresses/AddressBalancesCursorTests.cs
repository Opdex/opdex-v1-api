using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.Addresses
{
    public class AddressBalancesCursorTests
    {
        [Fact]
        public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new AddressBalancesCursor(Enumerable.Empty<string>(), false, false, SortDirectionType.ASC, 50 + 1, PagingDirection.Forward, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>("limit", Act);
        }

        [Fact]
        public void Create_InvalidPointer_ThrowArgumentException()
        {
            // Arrange
            // Act
            static void Act() => new AddressBalancesCursor(Enumerable.Empty<string>(), false, false, SortDirectionType.ASC, 25, PagingDirection.Forward, -1);

            // Assert
            Assert.Throws<ArgumentException>("pointer", Act);
        }

        [Fact]
        public void Create_NullTokensProvided_SetToEmpty()
        {
            // Arrange
            // Act
            var cursor = new AddressBalancesCursor(null, true, false, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Assert
            cursor.Tokens.Should().BeEmpty();
        }

        [Fact]
        public void ToString_StringifiesCursor_FormatCorrectly()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(new string[] { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", "P8bB9yPr3vVByqfmM5KXftyGckAtAdu6f8" }, true, false, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.ToString();

            // Assert
            result.Should().Contain("tokens:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;");
            result.Should().Contain("tokens:P8bB9yPr3vVByqfmM5KXftyGckAtAdu6f8;");
            result.Should().Contain("includeLpTokens:True;");
            result.Should().Contain("includeZeroBalances:False;");
            result.Should().Contain("direction:ASC;");
            result.Should().Contain("limit:25;");
            result.Should().Contain("paging:Forward;");
            result.Should().Contain("pointer:NTAw;");
        }

        [Fact]
        public void Turn_NonIdenticalPointer_ReturnAnotherCursor()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(new string[] { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L" }, true, false, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.Turn(PagingDirection.Backward, 567);

            // Assert
            result.Should().BeOfType<AddressBalancesCursor>();
            var adjacentCursor = (AddressBalancesCursor)result;
            adjacentCursor.Tokens.Should().BeEquivalentTo(cursor.Tokens);
            adjacentCursor.IncludeLpTokens.Should().Be(cursor.IncludeLpTokens);
            adjacentCursor.IncludeZeroBalances.Should().Be(cursor.IncludeZeroBalances);
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
        [InlineData("includeLpTokens:Maybe;includeZeroBalances:True;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // invalid includeLpTokens
        [InlineData("includeZeroBalances:True;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // missing includeLpTokens
        [InlineData("includeLpTokens:True;includeZeroBalances:Maybe;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // invalid includeZeroBalances
        [InlineData("includeLpTokens:True;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // missing includeZeroBalances
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:Invalid;limit:50;paging:Forward;pointer:NTAw;")] // invalid orderBy
        [InlineData("includeLpTokens:True;includeZeroBalances:False;limit:50;paging:Forward;pointer:NTAw;")] // missing orderBy
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:51;paging:Forward;pointer:NTAw;")] // over max limit
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;paging:Forward;pointer:NTAw;")] // missing limit
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:50;paging:Invalid;pointer:NTAw;")] // invalid paging direction
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:50;pointer:NTAw;")] // missing paging direction
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:LTE=;")] // pointer: -1;
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:YWJj")] // pointer: abc;
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:10")] // pointer not valid base64
        [InlineData("includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;")] // pointer missing
        public void TryParse_InvalidCursor_ReturnFalse(string stringified)
        {
            // Arrange
            // Act
            var canParse = AddressBalancesCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(false);
            cursor.Should().Be(null);
        }

        [Fact]
        public void TryParse_ValidCursor_ReturnTrue()
        {
            // Arrange
            var stringified = "tokens:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;includeLpTokens:True;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:MTA=;"; // pointer: 10;

            // Act
            var canParse = AddressBalancesCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(true);
            cursor.Tokens.Should().ContainSingle(token => token == "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L");
            cursor.IncludeLpTokens.Should().Be(true);
            cursor.IncludeZeroBalances.Should().Be(false);
            cursor.SortDirection.Should().Be(SortDirectionType.ASC);
            cursor.Limit.Should().Be(50);
            cursor.PagingDirection.Should().Be(PagingDirection.Forward);
            cursor.Pointer.Should().Be(10);
        }
    }
}
