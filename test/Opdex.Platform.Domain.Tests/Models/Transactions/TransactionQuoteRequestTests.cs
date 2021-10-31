using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Transactions
{
    public class TransactionQuoteRequestTests
    {
        [Fact]
        public void CreateNew_TransactionQuoteRequest_InvalidSender_ThrowArgumentNullException()
        {
            // Arrange
            Address sender = Address.Empty;
            Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Zero;
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            // Act
            void Act() => new TransactionQuoteRequest(sender, to, amount, method, callback);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(sender)} must not be null or empty.");
        }

        [Fact]
        public void CreateNew_TransactionQuoteRequest_InvalidTo_ThrowArgumentNullException()
        {
            // Arrange
            Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address to = Address.Empty;
            FixedDecimal amount = FixedDecimal.Zero;
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            // Act
            void Act() => new TransactionQuoteRequest(sender, to, amount, method, callback);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(to)} must not be null or empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_TransactionQuoteRequest_InvalidMethod_ThrowArgumentNullException(string method)
        {
            // Arrange
            Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Zero;
            const string callback = "https://dev-api.opdex.com/transactions";

            // Act
            void Act() => new TransactionQuoteRequest(sender, to, amount, method, callback);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(method)} must not be null or empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_TransactionQuoteRequest_InvalidCallback_ThrowArgumentNullException(string callback)
        {
            // Arrange
            Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Zero;
            const string method = "Swap";

            // Act
            void Act() => new TransactionQuoteRequest(sender, to, amount, method, callback);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(callback)} must not be null or empty.");
        }

        [Fact]
        public void CreateNew_TransactionQuoteRequest()
        {
            // Arrange
            Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.1");
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            var parameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", (UInt256)10)
            };

            // Act
            var request = new TransactionQuoteRequest(sender, to, amount, method, callback, parameters);

            // Assert
            request.Sender.Should().Be(sender);
            request.To.Should().Be(to);
            request.Amount.Should().Be(amount);
            request.Method.Should().Be(method);
            request.Callback.Should().Be(callback);
            request.Parameters.Should().BeEquivalentTo(parameters);
            request.MethodParameters.Should().BeEquivalentTo(new SmartContractMethodParameter((UInt256)10));
        }
    }
}
