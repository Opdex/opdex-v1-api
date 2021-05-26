using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Extensions;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Extensions
{
    public class StringExtensionTests
    {
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