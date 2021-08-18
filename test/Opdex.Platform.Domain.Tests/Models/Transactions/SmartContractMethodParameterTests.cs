using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Transactions
{
    public class SmartContractMethodParameterTests
    {
        [Fact]
        public void Create_BooleanParameter_Valid()
        {
            // Arrange
            bool value = false;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.Boolean);
        }

        [Fact]
        public void Create_ByteParameter_Valid()
        {
            // Arrange
            byte value = 111;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.Byte);
        }

        [Fact]
        public void Create_CharParameter_Valid()
        {
            // Arrange
            char value = '#';

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.Char);
        }

        [Fact]
        public void Create_StringParameter_Invalid()
        {
            // Arrange
            string value = null;

            // Act
            void Act() => new SmartContractMethodParameter(value);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void Create_StringParameter_Valid()
        {
            // Arrange
            string value = "Hello world";

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value);
            parameter.Type.Should().Be(SmartContractParameterType.String);
        }

        [Fact]
        public void Create_UInt32Parameter_Valid()
        {
            // Arrange
            uint value = 50;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.UInt32);
        }

        [Fact]
        public void Create_Int32Parameter_Valid()
        {
            // Arrange
            int value = 50;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.Int32);
        }

        [Fact]
        public void Create_UInt64Parameter_Valid()
        {
            // Arrange
            ulong value = 50;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.UInt64);
        }

        [Fact]
        public void Create_Int64Parameter_Valid()
        {
            // Arrange
            long value = 50;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.Int64);
        }

        [Fact]
        public void Create_AddressParameter_Invalid()
        {
            // Arrange
            Address value = Address.Empty;

            // Act
            void Act() => new SmartContractMethodParameter(value);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void Create_AddressParameter_Valid()
        {
            // Arrange
            Address value = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.Address);
        }

        [Fact]
        public void Create_ByteArrayParameter_Invalid()
        {
            // Arrange
            byte[] value = null;

            // Act
            void Act() => new SmartContractMethodParameter(value);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void Create_ByteArrayParameter_Valid()
        {
            // Arrange
            byte[] value = new byte[] { 100, 241, 53, 28, 4, 130, 130 };

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(BitConverter.ToString(value));
            parameter.Type.Should().Be(SmartContractParameterType.ByteArray);
        }

        [Fact]
        public void Create_UInt128Parameter_Valid()
        {
            // Arrange
            UInt128 value = 500;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.UInt128);
        }

        [Fact]
        public void Create_UInt256Parameter_Valid()
        {
            // Arrange
            UInt256 value = 500;

            // Act
            var parameter = new SmartContractMethodParameter(value);

            // Assert
            parameter.Value.Should().Be(value.ToString());
            parameter.Type.Should().Be(SmartContractParameterType.UInt256);
        }

        [Fact]
        public void Serialize()
        {
            // Arrange
            var parameter = new SmartContractMethodParameter(true);

            // Act
            var serialized = parameter.Serialize();

            // Assert
            serialized.Should().Be($"{(uint)parameter.Type}#{parameter.Value}");
        }

        [Fact]
        public void Deserialize()
        {
            // Arrange
            var address = new Address("PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy");
            var serialized = new SmartContractMethodParameter(address).Serialize();

            // Act
            var parameter = SmartContractMethodParameter.Deserialize(serialized);

            // Assert
            parameter.Type.Should().Be(SmartContractParameterType.Address);
            parameter.Value.Should().Be(address.ToString());
        }
    }
}
