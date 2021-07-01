using System;
using System.Numerics;
using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Opdex.Platform.Common.Tests
{
    public class StringExtensionsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StringExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        [InlineData("Test", true)]
        public void StringHasValue_Success(string value, bool expected)
        {
            value.HasValue().Should().Be(expected);
        }


        [Theory]
        [InlineData(8, "10000000000", "100.00000000")]
        [InlineData(8, "1000000000", "10.00000000")]
        [InlineData(8, "100000000", "1.00000000")]
        [InlineData(8, "10000000", "0.10000000")]
        [InlineData(8, "1000000", "0.01000000")]
        [InlineData(8, "100000", "0.00100000")]
        [InlineData(8, "10000", "0.00010000")]
        [InlineData(8, "1000", "0.00001000")]
        [InlineData(8, "100", "0.00000100")]
        [InlineData(8, "10", "0.00000010")]
        [InlineData(8, "1", "0.00000001")]
        public void StringInsertsDecimal_Success(int decimals, string value, string expected)
        {
            value.InsertDecimal(decimals).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, "100.0090000", "100.00")]
        [InlineData(3, "100.0090000", "100.009")]
        public void StringRound_Success(int precision, string value, string expected)
        {
            value.CutPrecisely(precision).Should().Be(expected);
        }

        [Theory]
        [InlineData("1.32", true)]
        [InlineData("1.32e", false)]
        [InlineData("132", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ValidDecimalNumber_Success(string value, bool expected)
        {
            value.IsValidDecimalNumber().Should().Be(expected);
        }

        [Theory]
        [InlineData(null, 100_000_000, 1.00, 0)]
        [InlineData(" ", 100_000_000, 1.00, 0)]
        [InlineData("", 100_000_000, 1.00, 0)]
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
            tokenAmount.TotalFiat(fiatPerToken, tokenSats).Should().Be(expectedPrice);
        }

        [Fact]
        public void TotalFiat_Throws_ArgumentOutOfRangeException()
        {
            const string tokenAmount = "1.25";

            tokenAmount.Invoking(t => t.TotalFiat(1.00m, TokenConstants.Cirrus.Sats))
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("Must be a valid numeric number. *");
        }

        [Fact]
        public void TotalFiat_Throws_Exception()
        {
            const string tokenAmount = "10000000000000000000000000000000000000000000000000000000000";

            tokenAmount.Invoking(t => t.TotalFiat(10000000000000000000000000.00m, 0))
                .Should()
                .Throw<OverflowException>();
        }

        [Theory]
        [InlineData(null, 100_000_000, 1.00, 0)]
        [InlineData(" ", 100_000_000, 1.00, 0)]
        [InlineData("", 100_000_000, 1.00, 0)]
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
            tokenAmount.FiatPerToken(fiatAmount, tokenSats).Should().Be(expectedPrice);
        }

        [Fact]
        public void FiatPerToken_Throws_ArgumentOutOfRangeException()
        {
            const string tokenAmount = "1.25";

            tokenAmount.Invoking(t => t.FiatPerToken(1.00m, TokenConstants.Cirrus.Sats))
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("Must be a valid numeric number. *");
        }

        [Fact]
        public void FiatPerToken_Throws_Exception()
        {
            const string tokenAmount = "10000000000000000000000000000000000000000000000000000000000";

            tokenAmount.Invoking(t => t.FiatPerToken(10000000000000000000000000.00m, 0))
                .Should()
                .Throw<OverflowException>();
        }

        [Theory]
        [InlineData("100000000", "50000000", 100_000_000, 100)]
        [InlineData("4", "3", 100_000_000, 33.33)]
        [InlineData("150000000", "50000000", 100_000_000, 200)]
        [InlineData("75000000", "50000000", 100_000_000, 50)]
        [InlineData("50000000", "100000000", 100_000_000, -50)]
        [InlineData("10000000", "100000000", 100_000_000, -90)]
        [InlineData("1000000000000000000", "500000000000000000", 1_000_000_000_000_000_000, 100)]
        public void PercentChangeSats_Success(string current, string previous, ulong tokenSats, decimal expected)
        {
            current.PercentChange(previous, tokenSats).Should().Be(expected);
        }

        [Theory]
        [InlineData(1.00, 2.00, -50)]
        [InlineData(2.00, 1.00, 100)]
        [InlineData(4.00, 3.00, 33.33)]
        public void PercentChangeDecimals_Success(decimal current, decimal previous, decimal expected)
        {
            current.PercentChange(previous).Should().Be(expected);
        }
    }
}
