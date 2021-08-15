using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.Transactions
{
    public class TransactionsCursorTests
    {
        [Fact]
        public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new TransactionsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                        Enumerable.Empty<TransactionEventType>(),
                                                        Enumerable.Empty<string>(), SortDirectionType.ASC,
                                                        50 + 1, PagingDirection.Forward, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>("limit", Act);
        }

        [Theory]
        [ClassData(typeof(InvalidLongPointerData))]
        public void Create_InvalidPointer_ThrowArgumentException(PagingDirection pagingDirection, long pointer)
        {
            // Arrange
            // Act
            void Act() => new TransactionsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                        Enumerable.Empty<TransactionEventType>(),
                                                        Enumerable.Empty<string>(), SortDirectionType.ASC,
                                                        25, pagingDirection, pointer);

            // Assert
            Assert.Throws<ArgumentException>("pointer", Act);
        }

        [Fact]
        public void Create_NullEventTypesProvided_SetToEmpty()
        {
            // Arrange
            // Act
            // Arrange
            var cursor = new TransactionsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                null, Enumerable.Empty<string>(),
                                                SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Assert
            cursor.EventTypes.Should().BeEmpty();
        }

        [Fact]
        public void Create_NullContractsProvided_SetToEmpty()
        {
            // Arrange
            // Act
            // Arrange
            var cursor = new TransactionsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                Enumerable.Empty<TransactionEventType>(), null,
                                                SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Assert
            cursor.Contracts.Should().BeEmpty();
        }

        [Fact]
        public void ToString_StringifiesCursor_FormatCorrectly()
        {
            // Arrange
            var cursor = new TransactionsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                new TransactionEventType[] { TransactionEventType.ClaimPendingDeployerOwnershipEvent, TransactionEventType.SwapEvent },
                                                new string[] { "PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u" },
                                                SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.ToString();

            // Assert
            result.Should().Contain("wallet:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;");
            result.Should().Contain("eventTypes:ClaimPendingDeployerOwnershipEvent;");
            result.Should().Contain("eventTypes:SwapEvent;");
            result.Should().Contain("contracts:PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5;");
            result.Should().Contain("contracts:PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u;");
            result.Should().Contain("direction:ASC;");
            result.Should().Contain("limit:25;");
            result.Should().Contain("paging:Forward;");
            result.Should().Contain("pointer:NTAw;");
        }

        [Fact]
        public void Turn_NonIdenticalPointer_ReturnAnotherCursor()
        {
            // Arrange
            var cursor = new TransactionsCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                                new TransactionEventType[] { TransactionEventType.ClaimPendingDeployerOwnershipEvent, TransactionEventType.SwapEvent },
                                                new string[] { "PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u" },
                                                SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.Turn(PagingDirection.Backward, 567);

            // Assert
            result.Should().BeOfType<TransactionsCursor>();
            var adjacentCursor = (TransactionsCursor)result;
            adjacentCursor.Wallet.Should().Be(cursor.Wallet);
            adjacentCursor.EventTypes.Should().BeEquivalentTo(cursor.EventTypes);
            adjacentCursor.Contracts.Should().BeEquivalentTo(cursor.Contracts);
            adjacentCursor.SortDirection.Should().Be(cursor.SortDirection);
            adjacentCursor.Limit.Should().Be(cursor.Limit);
            adjacentCursor.PagingDirection.Should().Be(PagingDirection.Backward);
            adjacentCursor.Pointer.Should().Be(567);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
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
            var canParse = TransactionsCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(false);
            cursor.Should().Be(null);
        }

        [Fact]
        public void TryParse_ValidCursor_ReturnTrue()
        {
            // Arrange
            var stringified = "wallet:PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5;eventTypes:AddLiquidityEvent;contracts:PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L;direction:ASC;limit:50;paging:Forward;pointer:MTA=;"; // pointer: 10;

            // Act
            var canParse = TransactionsCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(true);
            cursor.Wallet.Should().Be("PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5");
            cursor.EventTypes.Should().ContainSingle(eventType => eventType == TransactionEventType.AddLiquidityEvent);
            cursor.Contracts.Should().ContainSingle(contract => contract == "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L");
            cursor.SortDirection.Should().Be(SortDirectionType.ASC);
            cursor.Limit.Should().Be(50);
            cursor.PagingDirection.Should().Be(PagingDirection.Forward);
            cursor.Pointer.Should().Be(10);
        }
    }
}
