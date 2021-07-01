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
    }
}
