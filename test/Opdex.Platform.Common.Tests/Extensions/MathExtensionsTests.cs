using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Xunit;

namespace Opdex.Platform.Common.Tests.Extensions
{
    public class MathExtensionsTests
    {
                [Theory]
        [InlineData("0", 100_000_000, 1.00, 0)]
        [InlineData("100000000", 100_000_000, 1.00, 1.00)]
        [InlineData("111111111", 100_000_000, 1.00, 1.11111111)]
        [InlineData("100000000", 100_000_000, 1.20, 1.20)]
        [InlineData("100000000", 100_000_000, 2.00, 2.00)]
        [InlineData("150000000", 100_000_000, 5.50, 8.25)]
        [InlineData("200000000", 100_000_000, 10.11, 20.22)]
        [InlineData("300000000", 100_000_000, 10.11, 30.33)]
        [InlineData("2000000000000000000", 1_000_000_000_000_000_000, 50.00, 100.00)]
        [InlineData("2000000000000000000000", 1_000_000_000_000_000_000, 50.00, 100_000.00)]
        [InlineData("2000000000000000000000000", 1_000_000_000_000_000_000, 50.00, 100_000_000.00)]
        [InlineData("2000000000000000000000000000", 1_000_000_000_000_000_000, 50.00, 100_000_000_000.00)]
        [InlineData("2000000000000000000000000000000", 1_000_000_000_000_000_000, 50.00, 100_000_000_000_000.00)] // 2 trillion
        [InlineData("2000000000000000000000000000000000", 1_000_000_000_000_000_000, 50.00, 100_000_000_000_000_000.00)] // 2 quadrillion
        public void TotalFiat_Success(string tokenAmount, ulong tokenSats, decimal fiatPerToken, decimal expectedPrice)
        {
            MathExtensions.TotalFiat(UInt256.Parse(tokenAmount), fiatPerToken, tokenSats).Should().Be(expectedPrice);
        }

        [Theory]
        [InlineData("0", 100_000_000, 1.00, 0)]
        [InlineData("100000000", 100_000_000, 1.20, 1.20)]
        [InlineData("111111111", 100_000_000, 1.11111111, 1.00)]
        [InlineData("1500000000", 100_000_000, 15.00, 1.00)]
        [InlineData("200000000", 100_000_000, 30.00, 15.00)]
        [InlineData("300000000", 100_000_000, 30.33, 10.11)]
        [InlineData("2000000000000000000", 1_000_000_000_000_000_000, 100.00, 50.00)]
        [InlineData("2000000000000000000000", 1_000_000_000_000_000_000, 100_000.00, 50.00)]
        [InlineData("2000000000000000000000000", 1_000_000_000_000_000_000, 100_000_000.00, 50.00)]
        [InlineData("2000000000000000000000000000", 1_000_000_000_000_000_000, 100_000_000_000.00, 50.00)]
        [InlineData("2000000000000000000000000000000", 1_000_000_000_000_000_000, 100_000_000_000_000.00, 50.00)] // 2 trillion
        [InlineData("2000000000000000000000000000000000", 1_000_000_000_000_000_000, 100_000_000_000_000_000.00, 50.00)] // 2 quadrillion
        public void FiatPerToken_Success(string tokenAmount, ulong tokenSats, decimal fiatAmount, decimal expectedPrice)
        {
            MathExtensions.FiatPerToken(UInt256.Parse(tokenAmount), fiatAmount, tokenSats).Should().Be(expectedPrice);
        }

        [Theory]
        [InlineData("0", "50000000", 100_000_000, -100)]
        [InlineData("50000000", "0", 100_000_000, 0)]
        [InlineData("100000000", "50000000", 100_000_000, 100)]
        [InlineData("4", "3", 100_000_000, 33.33)]
        [InlineData("150000000", "50000000", 100_000_000, 200)]
        [InlineData("75000000", "50000000", 100_000_000, 50)]
        [InlineData("50000000", "100000000", 100_000_000, -50)]
        [InlineData("10000000", "100000000", 100_000_000, -90)]
        [InlineData("1000000000000000000", "500000000000000000", 1_000_000_000_000_000_000, 100)]
        public void PercentChangeSats_Success(string current, string previous, ulong tokenSats, decimal expected)
        {
            MathExtensions.PercentChange(UInt256.Parse(current), UInt256.Parse(previous), tokenSats).Should().Be(expected);
        }

        [Theory]
        [InlineData(1.00, 2.00, -50)]
        [InlineData(0.00, 2.00, -100)]
        [InlineData(2.00, 0.00, 0)]
        [InlineData(2.00, 1.00, 100)]
        [InlineData(4.00, 3.00, 33.33)]
        public void PercentChangeDecimals_Success(decimal current, decimal previous, decimal expected)
        {
            MathExtensions.PercentChange(current, previous).Should().Be(expected);
        }
    }
}
