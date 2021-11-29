using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.Tokens
{
    public class TokensCursorTests
    {
        [Fact]
        public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new TokensCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                  Enumerable.Empty<Address>(),
                                                  TokenProvisionalFilter.Provisional,
                                                  TokenOrderByType.PriceUsd,
                                                  SortDirectionType.ASC,
                                                  50 + 1,
                                                  PagingDirection.Forward,
                                                  ("0", 0));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>("limit", Act);
        }

        [Theory]
        [InlineData(PagingDirection.Backward, null, 0)]
        public void Create_InvalidPointer_ThrowArgumentException(PagingDirection pagingDirection, string orderByPointer, ulong pointer)
        {
            // Arrange
            // Act
            void Act() => new TokensCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                            Enumerable.Empty<Address>(),
                                            TokenProvisionalFilter.NonProvisional,
                                            TokenOrderByType.PriceUsd,
                                            SortDirectionType.ASC,
                                            25,
                                            pagingDirection,
                                            (orderByPointer, pointer));

            // Assert
            Assert.Throws<ArgumentException>("pointer", Act);
        }

        [Fact]
        public void Create_NullContractsProvided_SetToEmpty()
        {
            // Act
            // Arrange
            var cursor = new TokensCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                          null,
                                          TokenProvisionalFilter.Provisional,
                                          TokenOrderByType.PriceUsd,
                                          SortDirectionType.ASC,
                                          50,
                                          PagingDirection.Forward,
                                          ("50", 10));

            // Assert
            cursor.Tokens.Should().BeEmpty();
        }

        [Fact]
        public void ToString_StringifiesCursor_FormatCorrectly()
        {
            // Arrange
            var cursor = new TokensCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                          new Address[] { "PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u" },
                                          TokenProvisionalFilter.Provisional,
                                          TokenOrderByType.DailyPriceChangePercent,
                                          SortDirectionType.ASC,
                                          25,
                                          PagingDirection.Forward,
                                          ("50.00", 10));

            // Act
            var result = cursor.ToString();

            // Assert
            result.Should().Contain("keyword:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;");
            result.Should().Contain("provisional:Provisional;");
            result.Should().Contain("tokens:PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5;");
            result.Should().Contain("tokens:PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u;");
            result.Should().Contain("orderBy:DailyPriceChangePercent;");
            result.Should().Contain("direction:ASC;");
            result.Should().Contain("limit:25;");
            result.Should().Contain("paging:Forward;");
            result.Should().Contain("pointer:KDUwLjAwLCAxMCk=;");
        }

        [Fact]
        public void Turn_NonIdenticalPointer_ReturnAnotherCursor()
        {
            // Arrange
            var cursor = new TokensCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                          new Address[] { "PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u" },
                                          TokenProvisionalFilter.NonProvisional,
                                          TokenOrderByType.DailyPriceChangePercent,
                                          SortDirectionType.ASC,
                                          25,
                                          PagingDirection.Forward,
                                          ("50.00", 10));

            // Act
            var result = cursor.Turn(PagingDirection.Backward, ("100.00", 567));

            // Assert
            result.Should().BeOfType<TokensCursor>();
            var adjacentCursor = (TokensCursor)result;
            adjacentCursor.Keyword.Should().Be(cursor.Keyword);
            adjacentCursor.ProvisionalFilter.Should().Be(cursor.ProvisionalFilter);
            adjacentCursor.Tokens.Should().BeEquivalentTo(cursor.Tokens);
            adjacentCursor.OrderBy.Should().Be(cursor.OrderBy);
            adjacentCursor.SortDirection.Should().Be(cursor.SortDirection);
            adjacentCursor.Limit.Should().Be(cursor.Limit);
            adjacentCursor.PagingDirection.Should().Be(PagingDirection.Backward);
            adjacentCursor.Pointer.Should().Be(("100.00", 567));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData(";:;;;;;::;;;:::;;;:::;;:::;;")]
        [InlineData("direction:Invalid;limit:50;paging:Forward;pointer:NTAw;")] // invalid direction
        [InlineData("limit:50;paging:Forward;pointer:NTAw;")] // missing direction
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
            var canParse = TokensCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(false);
            cursor.Should().Be(null);
        }

        [Fact]
        public void TryParse_ValidCursor_ReturnTrue()
        {
            // Arrange
            var stringified = "keyword:PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5;orderBy:PriceUsd;provisional:NonProvisional;tokens:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;direction:ASC;limit:50;paging:Forward;pointer:KDUwLjAwLCAxMCk=;"; // pointer: 10;

            // Act
            var canParse = TokensCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(true);
            cursor.Keyword.Should().Be("PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5");
            cursor.OrderBy.Should().Be(TokenOrderByType.PriceUsd);
            cursor.ProvisionalFilter.Should().Be(TokenProvisionalFilter.NonProvisional);
            cursor.Tokens.Should().ContainSingle(t => t == "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L");
            cursor.SortDirection.Should().Be(SortDirectionType.ASC);
            cursor.Limit.Should().Be(50);
            cursor.PagingDirection.Should().Be(PagingDirection.Forward);
            cursor.Pointer.Should().Be(("50.00", 10));
        }
    }
}
