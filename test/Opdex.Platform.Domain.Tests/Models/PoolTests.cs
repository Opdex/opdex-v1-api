using FluentAssertions;
using Opdex.Platform.Domain.Models;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class PoolTests
    {
        [Fact]
        public void CreatePool_Success()
        {
            const string address = "Address";
            const long tokenId = 2;
            const long marketId = 1;

            var pool = new LiquidityPool(address, tokenId, marketId);

            pool.Id.Should().Be(0);
            pool.Address.Should().Be(address);
            pool.TokenId.Should().Be(tokenId);
        }
    }
}