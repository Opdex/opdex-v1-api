using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Models.Requests.MiningPools
{
    public class MiningPoolFilterParametersTests
    {
        [Fact]
        public void DefaultPropertyValues()
        {
            // Arrange
            // Act
            var filters = new MiningPoolFilterParameters();

            // Assert
            filters.LiquidityPools.Should().BeEmpty();
            filters.MiningStatus.Should().Be(MiningStatusFilter.Any);
            filters.Direction.Should().Be(default(SortDirectionType));
            filters.Limit.Should().Be(default);
        }

        [Fact]
        public void BuildCursor_CursorStringNotProvided_ReturnFiltered()
        {
            // Arrange
            var filters = new MiningPoolFilterParameters
            {
                LiquidityPools = new Address[] { new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm") },
                MiningStatus = MiningStatusFilter.Inactive,
                Limit = 20,
                Direction = SortDirectionType.DESC
            };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.LiquidityPools.Should().BeEquivalentTo(filters.LiquidityPools);
            cursor.MiningStatus.Should().Be(filters.MiningStatus);
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
            var filters = new MiningPoolFilterParameters { EncodedCursor = "NOT_BASE_64_****" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_NotAValidCursorString_ReturnNull()
        {
            // Arrange
            var filters = new MiningPoolFilterParameters { EncodedCursor = "Tk9UX1ZBTElE" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_ValidCursorString_ReturnCursor()
        {
            // Arrange
            var filters = new MiningPoolFilterParameters { EncodedCursor = "bWluaW5nU3RhdHVzOkFueTtkaXJlY3Rpb246REVTQztsaW1pdDo1O3BhZ2luZzpGb3J3YXJkO3BvaW50ZXI6TXc9PTs=" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().NotBe(null);
        }
    }
}
