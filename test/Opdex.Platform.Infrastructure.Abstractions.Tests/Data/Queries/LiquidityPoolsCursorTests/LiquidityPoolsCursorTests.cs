using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.LiquidityPoolsCursorTests
{
    public class LiquidityPoolsCursorTests
    {
        [Fact]
        public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new LiquidityPoolsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                          Enumerable.Empty<Address>(),
                                                          Enumerable.Empty<Address>(),
                                                          Enumerable.Empty<Address>(),
                                                          LiquidityPoolStakingStatusFilter.Any,
                                                          LiquidityPoolNominationStatusFilter.Any,
                                                          LiquidityPoolMiningStatusFilter.Any,
                                                          LiquidityPoolOrderByType.Any,
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
            void Act() => new LiquidityPoolsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                   Enumerable.Empty<Address>(),
                                                   Enumerable.Empty<Address>(),
                                                   Enumerable.Empty<Address>(),
                                                   LiquidityPoolStakingStatusFilter.Any,
                                                   LiquidityPoolNominationStatusFilter.Any,
                                                   LiquidityPoolMiningStatusFilter.Any,
                                                   LiquidityPoolOrderByType.Any,
                                                   SortDirectionType.ASC,
                                                   25,
                                                   pagingDirection,
                                                   (orderByPointer, pointer));

            // Assert
            Assert.Throws<ArgumentException>("pointer", Act);
        }

        [Fact]
        public void Create_NullMarketsProvided_SetToEmpty()
        {
            // Act
            // Arrange
            var cursor = new LiquidityPoolsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                  null,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  ("0", 10));

            // Assert
            cursor.Markets.Should().BeEmpty();
        }

        [Fact]
        public void Create_NullLiquidityPoolsProvided_SetToEmpty()
        {
            // Act
            // Arrange
            var cursor = new LiquidityPoolsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                  Enumerable.Empty<Address>(),
                                                  null,
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  ("0", 10));

            // Assert
            cursor.LiquidityPools.Should().BeEmpty();
        }

        [Fact]
        public void Create_NullTokensProvided_SetToEmpty()
        {
            // Act
            // Arrange
            var cursor = new LiquidityPoolsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  null,
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  ("0", 10));

            // Assert
            cursor.Tokens.Should().BeEmpty();
        }

        [Fact]
        public void ToString_StringifiesCursor_FormatCorrectly()
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                  new List<Address> { "PUMpPykkfL3XhYPefjjc9U4kqdSqkCrc4L"},
                                                  new List<Address> { "Pfjjc9U4kqdSqkUMpPykkfL3XhYPeCrc4L"},
                                                  new List<Address> { "PU4kqdSqkCrc4LUMpPykkfL3XhYPefjjc9"},
                                                  LiquidityPoolStakingStatusFilter.Enabled,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Disabled,
                                                  LiquidityPoolOrderByType.Liquidity,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  ("50.00", 10));

            // Act
            var result = cursor.ToString();

            // Assert
            result.Should().Contain("keyword:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;");
            result.Should().Contain("markets:PUMpPykkfL3XhYPefjjc9U4kqdSqkCrc4L;");
            result.Should().Contain("liquidityPools:Pfjjc9U4kqdSqkUMpPykkfL3XhYPeCrc4L;");
            result.Should().Contain("tokens:PU4kqdSqkCrc4LUMpPykkfL3XhYPefjjc9;");
            result.Should().Contain("stakingFilter:Enabled;");
            result.Should().Contain("nominationFilter:Any;");
            result.Should().Contain("miningFilter:Disabled;");
            result.Should().Contain("orderBy:Liquidity;");
            result.Should().Contain("direction:ASC;");
            result.Should().Contain("limit:25;");
            result.Should().Contain("paging:Forward;");
            result.Should().Contain("pointer:KDUwLjAwLCAxMCk=;");
        }

        [Fact]
        public void Turn_NonIdenticalPointer_ReturnAnotherCursor()
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                  new List<Address> { "PUMpPykkfL3XhYPefjjc9U4kqdSqkCrc4L"},
                                                  new List<Address> { "Pfjjc9U4kqdSqkUMpPykkfL3XhYPeCrc4L"},
                                                  new List<Address> { "PU4kqdSqkCrc4LUMpPykkfL3XhYPefjjc9"},
                                                  LiquidityPoolStakingStatusFilter.Enabled,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Disabled,
                                                  LiquidityPoolOrderByType.Liquidity,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  ("50.00", 10));

            // Act
            var result = cursor.Turn(PagingDirection.Backward, ("100.00", 567));

            // Assert
            result.Should().BeOfType<LiquidityPoolsCursor>();
            var adjacentCursor = (LiquidityPoolsCursor)result;
            adjacentCursor.Keyword.Should().Be(cursor.Keyword);
            adjacentCursor.Markets.Should().BeEquivalentTo(cursor.Markets);
            adjacentCursor.LiquidityPools.Should().BeEquivalentTo(cursor.LiquidityPools);
            adjacentCursor.Tokens.Should().BeEquivalentTo(cursor.Tokens);
            adjacentCursor.StakingFilter.Should().Be(cursor.StakingFilter);
            adjacentCursor.MiningFilter.Should().Be(cursor.MiningFilter);
            adjacentCursor.NominationFilter.Should().Be(cursor.NominationFilter);
            adjacentCursor.OrderBy.Should().BeEquivalentTo(cursor.OrderBy);
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
            var canParse = LiquidityPoolsCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(false);
            cursor.Should().Be(null);
        }

        [Fact]
        public void TryParse_ValidCursor_ReturnTrue()
        {
            // Arrange
            var stringified = "keyword:PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5;orderBy:Name;stakingFilter:Enabled;miningFilter:Any;nominationFilter:NonNominated;markets:PL3XhYPefjjc9U4kSqkCUMpPykkfqdrc4L;liquidityPools:PqkCUMpPykkfqdrc4LL3XhYPefjjc9U4kS;tokens:PXKprGLzzUCPT9Wqu5AmvCGQNeVVDMbgUk;direction:ASC;limit:50;paging:Forward;pointer:KDUwLjAwLCAxMCk=;"; // pointer: 10;

            // Act
            var canParse = LiquidityPoolsCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(true);
            cursor.Keyword.Should().Be("PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5");
            cursor.OrderBy.Should().Be(LiquidityPoolOrderByType.Name);
            cursor.StakingFilter.Should().Be(LiquidityPoolStakingStatusFilter.Enabled);
            cursor.NominationFilter.Should().Be(LiquidityPoolNominationStatusFilter.NonNominated);
            cursor.MiningFilter.Should().Be(LiquidityPoolMiningStatusFilter.Any);
            cursor.Markets.Should().ContainSingle(m => m == "PL3XhYPefjjc9U4kSqkCUMpPykkfqdrc4L");
            cursor.LiquidityPools.Should().ContainSingle(lp => lp == "PqkCUMpPykkfqdrc4LL3XhYPefjjc9U4kS");
            cursor.Tokens.Should().ContainSingle(t => t == "PXKprGLzzUCPT9Wqu5AmvCGQNeVVDMbgUk");
            cursor.SortDirection.Should().Be(SortDirectionType.ASC);
            cursor.Limit.Should().Be(50);
            cursor.PagingDirection.Should().Be(PagingDirection.Forward);
            cursor.Pointer.Should().Be(("50.00", 10));
        }
    }
}
