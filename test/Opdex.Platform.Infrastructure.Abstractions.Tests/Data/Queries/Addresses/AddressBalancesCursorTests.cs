using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.Addresses;

public class AddressBalancesCursorTests
{
    [Fact]
    public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => new AddressBalancesCursor(Enumerable.Empty<Address>(), TokenProvisionalFilter.All, false, SortDirectionType.ASC, 50 + 1, PagingDirection.Forward, 0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>("limit", Act);
    }

    [Theory]
    [ClassData(typeof(InvalidLongPointerData))]
    public void Create_InvalidPointer_ThrowArgumentException(PagingDirection pagingDirection, ulong pointer)
    {
        // Arrange
        // Act
        void Act() => new AddressBalancesCursor(Enumerable.Empty<Address>(), TokenProvisionalFilter.All, false, SortDirectionType.ASC, 25, pagingDirection, pointer);

        // Assert
        Assert.Throws<ArgumentException>("pointer", Act);
    }

    [Fact]
    public void Create_NullTokensProvided_SetToEmpty()
    {
        // Arrange
        // Act
        var cursor = new AddressBalancesCursor(null, TokenProvisionalFilter.All, false, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

        // Assert
        cursor.Tokens.Should().BeEmpty();
    }

    [Fact]
    public void ToString_StringifiesCursor_FormatCorrectly()
    {
        // Arrange
        var cursor = new AddressBalancesCursor(new Address[] { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", "P8bB9yPr3vVByqfmM5KXftyGckAtAdu6f8" }, TokenProvisionalFilter.Provisional, false, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

        // Act
        var result = cursor.ToString();

        // Assert
        result.Should().Contain("tokens:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;");
        result.Should().Contain("tokens:P8bB9yPr3vVByqfmM5KXftyGckAtAdu6f8;");
        result.Should().Contain("tokenType:Provisional;");
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
        var cursor = new AddressBalancesCursor(new Address[] { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L" }, TokenProvisionalFilter.Provisional, false, SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

        // Act
        var result = cursor.Turn(PagingDirection.Backward, 567);

        // Assert
        result.Should().BeOfType<AddressBalancesCursor>();
        var adjacentCursor = (AddressBalancesCursor)result;
        adjacentCursor.Tokens.Should().BeEquivalentTo(cursor.Tokens);
        adjacentCursor.TokenType.Should().Be(cursor.TokenType);
        adjacentCursor.IncludeZeroBalances.Should().Be(cursor.IncludeZeroBalances);
        adjacentCursor.SortDirection.Should().Be(cursor.SortDirection);
        adjacentCursor.Limit.Should().Be(cursor.Limit);
        adjacentCursor.PagingDirection.Should().Be(PagingDirection.Backward);
        adjacentCursor.Pointer.Should().Be(567);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespaceStringData))]
    [InlineData(";:;;;;;::;;;:::;;;:::;;:::;;")]
    [InlineData("tokenType:Unknown;includeZeroBalances:True;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // invalid tokenType
    [InlineData("includeZeroBalances:True;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // missing tokenType
    [InlineData("tokenType:All;includeZeroBalances:Maybe;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // invalid includeZeroBalances
    [InlineData("tokenType:All;direction:ASC;limit:50;paging:Forward;pointer:NTAw;")] // missing includeZeroBalances
    [InlineData("tokenType:All;includeZeroBalances:False;direction:Invalid;limit:50;paging:Forward;pointer:NTAw;")] // invalid orderBy
    [InlineData("tokenType:All;includeZeroBalances:False;limit:50;paging:Forward;pointer:NTAw;")] // missing orderBy
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;limit:51;paging:Forward;pointer:NTAw;")] // over max limit
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;paging:Forward;pointer:NTAw;")] // missing limit
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;limit:50;paging:Invalid;pointer:NTAw;")] // invalid paging direction
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;limit:50;pointer:NTAw;")] // missing paging direction
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:LTE=;")] // pointer: -1;
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:YWJj")] // pointer: abc;
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:10")] // pointer not valid base64
    [InlineData("tokenType:All;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;")] // pointer missing
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
        var stringified = "tokens:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;tokenType:NonProvisional;includeZeroBalances:False;direction:ASC;limit:50;paging:Forward;pointer:MTA=;"; // pointer: 10;

        // Act
        var canParse = AddressBalancesCursor.TryParse(stringified, out var cursor);

        // Assert
        canParse.Should().Be(true);
        cursor.Tokens.Should().ContainSingle(token => token == "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L");
        cursor.TokenType.Should().Be(TokenProvisionalFilter.NonProvisional);
        cursor.IncludeZeroBalances.Should().Be(false);
        cursor.SortDirection.Should().Be(SortDirectionType.ASC);
        cursor.Limit.Should().Be(50);
        cursor.PagingDirection.Should().Be(PagingDirection.Forward);
        cursor.Pointer.Should().Be(10);
    }
}