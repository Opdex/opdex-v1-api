using FluentAssertions;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Extensions;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.CirrusFullNodeApiTests.Extensions
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("53796E634576656E74", "ReservesLog")]
        [InlineData("5472616E736665724C6F67", "TransferLog")]
        [InlineData("5472616E736665724576656E74", "TransferLog")]
        [InlineData("4D696E744576656E74", "MintLog")]
        public void HexToString_Success(string hex, string expected)
        {
            hex.HexToString().Should().Be(expected);
        }
    }
}