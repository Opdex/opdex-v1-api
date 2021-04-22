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
    }
}