using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Transactions
{
    public class TransactionQuoteRequestParametersTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNewDecoded_TransactionQuoteRequestParameter_InvalidLabel_ThrowArgumentNullException(string label)
        {
            // Arrange
            // Act
            void Act() => new TransactionQuoteRequestParameter(label, "10", SmartContractParameterType.UInt256);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(label)} must not be null or empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNewDecoded_TransactionQuoteRequestParameter_InvalidValue_ThrowArgumentNullException(string value)
        {
            // Arrange
            // Act
            void Act() => new TransactionQuoteRequestParameter("Label", value, SmartContractParameterType.UInt256);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(value)} must not be null or empty.");
        }

        [Fact]
        public void CreateNewDecoded_TransactionQuoteRequestParameter_InvalidType_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const SmartContractParameterType type = (SmartContractParameterType)100;

            // Act
            void Act() => new TransactionQuoteRequestParameter("Label", "10", type);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(type)} is not valid.");
        }

        [Fact]
        public void CreateNewDecoded_TransactionQuoteRequestParameter()
        {
            // Arrange
            const string label = "Amount";
            const string value = "10";
            const SmartContractParameterType type = SmartContractParameterType.UInt256;

            // Act
            var parameter = new TransactionQuoteRequestParameter(label, value, type);

            // Assert
            parameter.Label.Should().Be(label);
            parameter.Value.Should().Be(value);
            parameter.Type.Should().Be(type);
            parameter.Serialized.Should().Be(value.ToSmartContractParameter(SmartContractParameterType.UInt256));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNewEncoded_TransactionQuoteRequestParameter_InvalidLabel_ThrowArgumentNullException(string label)
        {
            // Arrange
            // Act
            void Act() => new TransactionQuoteRequestParameter(label, "12#10");

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(label)} must not be null or empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("asdf")]
        public void CreateNewEncoded_TransactionQuoteRequestParameter_InvalidValue_ThrowArgumentException(string value)
        {
            // Arrange
            // Act
            void Act() => new TransactionQuoteRequestParameter("label", value);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Invalid parameter value");
        }

        [Theory]
        [InlineData("B#10")]
        [InlineData("1000#Some String")]
        [InlineData(" #test")]
        [InlineData(" # ")]
        [InlineData("#")]
        public void CreateNewEncoded_TransactionQuoteRequestParameter_InvalidType_ThrowException(string value)
        {
            // Arrange
            // Act
            void Act() => new TransactionQuoteRequestParameter("label", value);

            // Assert
            Assert.Throws<Exception>(Act).Message.Should().Contain("Unable to parse parameter type.");
        }

        [Fact]
        public void CreateNewEncoded_TransactionQuoteRequestParameter()
        {
            // Arrange
            const string label = "Amount";
            const string encodedValue = "12#10";
            const string expectedValue = "10";
            const SmartContractParameterType expectedType = SmartContractParameterType.UInt256;

            // Act
            var parameter = new TransactionQuoteRequestParameter(label, encodedValue);

            // Assert
            parameter.Label.Should().Be(label);
            parameter.Value.Should().Be(expectedValue);
            parameter.Type.Should().Be(expectedType);
            parameter.Serialized.Should().Be(encodedValue);
        }
    }
}
