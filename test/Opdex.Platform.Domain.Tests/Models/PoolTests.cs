using FluentAssertions;
using Opdex.Platform.Domain.Models.Pools;
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
            const ulong createdBlock = 3;
            const ulong modifiedBlock = 4;

            var pool = new LiquidityPool(address, tokenId, marketId, createdBlock, modifiedBlock);

            pool.Id.Should().Be(0);
            pool.Address.Should().Be(address);
            pool.TokenId.Should().Be(tokenId);
            pool.CreatedBlock.Should().Be(createdBlock);
            pool.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}