using FluentAssertions;
using Opdex.Core.Common.Extensions;
using Xunit;

namespace Opdex.Core.Common.Tests
{
    public class StringExtensionsTests
    {
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
        [InlineData("53796E634576656E74", "SyncEvent")]
        [InlineData("5472616E736665724C6F67", "TransferLog")]
        [InlineData("5472616E736665724576656E74", "TransferEvent")]
        [InlineData("4D696E744576656E74", "MintEvent")]
        public void HexToString_Success(string hex, string expected)
        {
            hex.HexToString().Should().Be(expected);
        }
    }
}