using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Numerics;
using Xunit;

namespace Opdex.Platform.Common.Tests.Extensions
{
    public class SatoshiConverterExtensionTests
    {
        [Theory]
        [InlineData(123456789, 8, 1.23456789)]
        [InlineData(987, 0, 987)]
        [InlineData(123456789, 4, 12345.6789)]
        public void ConvertsSatoshisToDecimal_Success(ulong satoshis, int decimals, decimal expected)
        {
            var value = satoshis.ToDecimal(decimals);

            value.Should().Be(expected);
        }

        [Theory]
        [InlineData(123456789, 8, 1.23456789)]
        [InlineData(987, 0, 987)]
        [InlineData(123456789, 4, 12345.6789)]
        public void ConvertsDecimalToSatoshis_Success(ulong expected, int decimals, decimal decimalValue)
        {
            var value = decimalValue.ToSatoshis(decimals);

            value.Should().Be(expected);
        }

        [Theory]
        [InlineData("10012000000", "100.12", 8)]
        [InlineData("12000000", "0.12", 8)]
        [InlineData("1", "0.00000001", 8)]
        [InlineData("0", "0.00000001", 1)]
        public void ConvertDecimalAsStringToSatoshis_Success(string expected, string value, int decimals)
        {
            value.ToSatoshis(decimals).Should().Be(expected);
        }

        [Fact]
        public void ToSatoshis_PrecisionGreaterThan18_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var value = FixedDecimal.Parse("1000.0000");

            // Act
            void Act() => value.ToSatoshis(19);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>("decimals", Act);
        }

        [Fact]
        public void ToSatoshis_GreaterThanUInt256MaxValue_ThrowOverflowException()
        {
            // Arrange
            var value = new FixedDecimal(((BigInteger)UInt256.MaxValue) + 1, 0);

            // Act
            void Act() => value.ToSatoshis(8);

            // Assert
            Assert.Throws<OverflowException>(Act);
        }

        [Theory]
        [InlineData("0.00000000", 8, 0)]
        [InlineData("0", 8, 0)]
        [InlineData("1234.0000", 4, 12340000)]
        [InlineData("0.4321", 18, 432100000000000000)]
        public void ToSatoshis_FromDecimal_CorrectPrecision(string input, byte decimals, ulong expected)
        {
            // Arrange
            var value = FixedDecimal.Parse(input);

            // Act
            var result = value.ToSatoshis(decimals);

            // Assert
            result.Should().Be((UInt256)expected);
        }

        [Theory]
        [InlineData(0, 8, "0.00000000")]
        [InlineData(12345, 0, "12345")]
        [InlineData(12345, 2, "123.45")]
        [InlineData(12345, 5, "0.12345")]
        [InlineData(12345, 18, "0.000000000000012345")]
        public void ToDecimal_FromSatoshis_CorrectPrecision(ulong input, byte decimals, string expected)
        {
            // Arrange
            var value = (UInt256)input;

            // Act
            var result = value.ToDecimal(decimals);

            // Assert
            result.Precision.Should().Be(decimals);
            result.ToString().Should().Be(expected);
        }
    }
}
