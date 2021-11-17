using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Models.Requests.Vaults
{
    public class VaultFilterParametersTests
    {
        [Fact]
        public void DefaultPropertyValues()
        {
            // Arrange
            // Act
            var filters = new VaultFilterParameters();

            // Assert
            filters.LockedToken.Should().Be(Address.Empty);
            filters.Direction.Should().Be(default(SortDirectionType));
            filters.Limit.Should().Be(default);
        }

        [Fact]
        public void BuildCursor_CursorStringNotProvided_ReturnFiltered()
        {
            // Arrange
            var filters = new VaultFilterParameters
            {
                LockedToken = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm"),
                Limit = 20,
                Direction = SortDirectionType.DESC
            };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.LockedToken.Should().Be(filters.LockedToken);
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
            var filters = new VaultFilterParameters { EncodedCursor = "NOT_BASE_64_****" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_NotAValidCursorString_ReturnNull()
        {
            // Arrange
            var filters = new VaultFilterParameters { EncodedCursor = "Tk9UX1ZBTElE" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_ValidCursorString_ReturnCursor()
        {
            // Arrange
            var filters = new VaultFilterParameters { EncodedCursor = "ZGlyZWN0aW9uOkRFU0M7bGltaXQ6NTtwYWdpbmc6Rm9yd2FyZDtwb2ludGVyOk13PT07" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().NotBe(null);
        }
    }
}
