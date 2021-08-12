using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Transactions
{
    public class TransactionQuoteRequestTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_TransactionQuoteRequest_InvalidSender_ThrowArgumentNullException(string sender)
        {
            // Arrange
            const string to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "0";
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            // Act
            void Act() => new TransactionQuoteRequest(sender, to, amount, method, callback);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(sender)} must not be null or empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_TransactionQuoteRequest_InvalidTo_ThrowArgumentNullException(string to)
        {
            // Arrange
            const string sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string amount = "0";
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
            const string sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "0";
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
            const string sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "0";
            const string method = "Swap";

            // Act
            void Act() => new TransactionQuoteRequest(sender, to, amount, method, callback);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(callback)} must not be null or empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1")]
        [InlineData("asdf")]
        public void CreateNew_TransactionQuoteRequest_InvalidAmount_ThrowArgumentException(string amount)
        {
            // Arrange
            const string sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            // Act
            void Act() => new TransactionQuoteRequest(sender, to, amount, method, callback);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain($"{nameof(amount)} must be a valid decimal number");
        }

        [Fact]
        public void CreateNew_TransactionQuoteRequest()
        {
            // Arrange
            const string sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "1.1";
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            var parameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", "10", SmartContractParameterType.UInt256)
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
            request.SerializedParameters.Should().BeEquivalentTo("12#10");
        }
    }
}
