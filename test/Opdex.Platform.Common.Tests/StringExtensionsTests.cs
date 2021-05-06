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
    }
}