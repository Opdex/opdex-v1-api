using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Transactions;
using System.Linq;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Models.Requests.Transactions
{
    public class TransactionFilterParametersTests
    {
        [Fact]
        public void DefaultPropertyValues()
        {
            // Arrange
            // Act
            var filters = new TransactionFilterParameters();

            // Assert
            filters.Wallet.Should().Be(Address.Empty);
            filters.Contracts.Should().BeEquivalentTo(Enumerable.Empty<Address>());
            filters.EventTypes.Should().BeEquivalentTo(Enumerable.Empty<TransactionEventType>());
            filters.Direction.Should().Be(default(SortDirectionType));
            filters.Limit.Should().Be(default);
        }

        [Fact]
        public void BuildCursor_CursorStringNotProvided_ReturnFiltered()
        {
            // Arrange
            var filters = new TransactionFilterParameters
            {
                Wallet = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm"),
                Contracts = new Address[] { new Address("tSJ9RArXP9BNJ8dV73owwyQjStr3ZMoHVC") },
                EventTypes = new TransactionEventType[] { TransactionEventType.ChangeMarketPermissionEvent, TransactionEventType.CreateVaultCertificateEvent },
                Limit = 20,
                Direction = SortDirectionType.DESC
            };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Wallet.Should().Be(filters.Wallet);
            cursor.Contracts.Should().BeEquivalentTo(filters.Contracts);
            cursor.EventTypes.Should().BeEquivalentTo(filters.EventTypes);
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
            var filters = new TransactionFilterParameters { EncodedCursor = "NOT_BASE_64_****" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_NotAValidCursorString_ReturnNull()
        {
            // Arrange
            var filters = new TransactionFilterParameters { EncodedCursor = "Tk9UX1ZBTElE" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().Be(null);
        }

        [Fact]
        public void BuildCursor_ValidCursorString_ReturnCursor()
        {
            // Arrange
            var filters = new TransactionFilterParameters { EncodedCursor = "ZGlyZWN0aW9uOkRFU0M7bGltaXQ6NTtwYWdpbmc6Rm9yd2FyZDtwb2ludGVyOk13PT07" };

            // Act
            var cursor = filters.BuildCursor();

            // Assert
            cursor.Should().NotBe(null);
        }
    }
}
