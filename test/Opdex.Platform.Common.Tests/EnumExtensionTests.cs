using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Xunit;

namespace Opdex.Platform.Common.Tests
{
    public class EnumExtensionTests
    {
        [Theory]
        [InlineData(NetworkType.DEVNET, true)]
        [InlineData(NetworkType.MAINNET, true)]
        [InlineData(NetworkType.TESTNET, true)]
        [InlineData((NetworkType)0, false)]
        [InlineData((NetworkType)5, false)]
        public void Enum_IsValid_Success(NetworkType network, bool validExpectation)
        {
            network.IsValid().Should().Be(validExpectation);
        }
    }
}
