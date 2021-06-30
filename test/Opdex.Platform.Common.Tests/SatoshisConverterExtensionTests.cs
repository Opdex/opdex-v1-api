using FluentAssertions;
using Xunit;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Common.Tests
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

        [Theory]
        [InlineData("123456789", 1.2345, 4, 8)] // Todo: doesn't actually round
        [InlineData("1", 0.0000, 4, 8)]
        [InlineData("50251", 0.0005, 4, 8)]
        [InlineData("50251", 0.00050251, 8, 8)]
        public void ToRoundedDecimal_Success(string value, decimal expected, int precision, int decimals)
        {
            value.ToRoundedDecimal(precision, decimals).Should().Be(expected);
        }
    }
}
