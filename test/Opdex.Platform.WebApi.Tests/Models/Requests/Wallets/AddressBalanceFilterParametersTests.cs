using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.WebApi.Models.Requests.Wallets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Models.Requests.Wallets
{
    public class AddressBalanceFilterParametersTests
    {
        [Fact]
        public void DefaultPropertyValues()
        {
            // Arrange
            // Act
            var filters = new AddressBalanceFilterParameters();

            // Assert
            filters.Tokens.Should().BeEmpty();
            filters.TokenType.Should().Be(TokenProvisionalFilter.All);
            filters.IncludeZeroBalances.Should().Be(false);
            filters.Direction.Should().Be(default(SortDirectionType));
            filters.Limit.Should().Be(default);
        }

        [Fact]
        public void BuildCursor_CursorStringNotProvided_ReturnFiltered()
        {
            // Arrange
            var filters = new AddressBalanceFilterParameters
            {
                Tokens = new Address[] { new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm") },
                TokenType = TokenProvisionalFilter.Provisional,
                IncludeZeroBalances = true,
                Limit = 20,
                Direction = SortDirectionType.DESC
            };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Tokens.Should().BeEquivalentTo(filters.Tokens);
            cursor.TokenType.Should().Be(filters.TokenType);
            cursor.IncludeZeroBalances.Should().Be(filters.IncludeZeroBalances);
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
            var filters = new AddressBalanceFilterParameters { EncodedCursor = "NOT_BASE_64_****" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_NotAValidCursorString_ReturnNull()
        {
            // Arrange
            var filters = new AddressBalanceFilterParameters { EncodedCursor = "Tk9UX1ZBTElE" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_ValidCursorString_ReturnCursor()
        {
            // Arrange
            var filters = new AddressBalanceFilterParameters { EncodedCursor = "ZGlyZWN0aW9uOkRFU0M7bGltaXQ6MjtwYWdpbmc6Rm9yd2FyZDt0b2tlblR5cGU6UHJvdmlzaW9uYWw7aW5jbHVkZVplcm9CYWxhbmNlczpGYWxzZTtwb2ludGVyOk9EWT07" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().NotBe(null);
        }
    }
}
